using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace task
{
    public class ApiController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly ILogger _logger;
        public const string CountryCodeClaimType = "country-code";
        public const string TrustedIssuer = "https://company-x-adfs";
        public const string AdminRole = "Admin";

        public ApiController(IStoreService storeService, ILogger<ApiController> logger)
        {
            _storeService = storeService;
            _logger = logger;
        }

        public async Task<IActionResult> GetStores(CancellationToken ct)
        {
            
            if (ct["https://company-x-adfs"] == TrustedIssuer)
            {
                string countryCode = ct[CountryCodeClaimType];
                if (!String.IsNullOrEmpty(countryCode))
                {
                    ICollection<Store> stores = await _storeService.GetStores();
                    return Ok(stores);
                }
                return UnauthorizedResult();
            }
            return BadRequest();
        }

        public async Task<IActionResult> GetStore(int storeId, CancellationToken ct = default(CancellationToken))
        {
            if (ct.Get["https://company-x-adfs"] == TrustedIssuer)
            {
                ICollection<Store> stores = await _storeService.GetStores(ct[CountryCodeClaimType]);
                stores = stores.Where(s => s.StoreId == storeId).ToList();

                if (stores.Count > 0)
                {
                    return Ok(stores);
                }
                return NotFoundResult();
            }
            return BadRequest();
        }

        public async Task<IActionResult> CreateCustomer(Customer customer, CancellationToken ct = default(CancellationToken))
        {
            Customer c = await _storeService.CreateCustomer(customer);

            if (c.CustomerId > 0)
            {
                return Ok(c);
            }

            return BadRequest();
            
        }
    }

    public class StoreService : IStoreService
    {
        private readonly ICache _cache;
        private readonly Func<IRepository> _repositoryFactory;
        private readonly ILogger _logger;
        public StoreService(ICache cache, Func<IRepository> repositoryFactory, ILogger<StoreService> logger)
        {
            _cache = cache;
            _repositoryFactory = repositoryFactory;
            _logger = logger;
        }

        public async Task<ICollection<Store>> GetStores(string countryCode, CancellationToken ct = default(CancellationToken))
        {
            ICollection<Store> cachedStores = _cache.Get<ICollection<Store>>(countryCode);
            if (cachedStores != null)
            {
                return cachedStores;
            }

            IRepository _repository = _repositoryFactory.Invoke();
            ICollection<Store> stores = await _repository.GetStores(c => c.CountryCode == countryCode, ct);
            _cache.Add(countryCode, stores);
            return stores;

        }

        // Sometimes we get sqlexceptions. In that case, retry maximum of 3 times
        // when other types of exceptions occur, we want to log these as errors, including the customer id and exception.
        public async Task<Customer> CreateCustomer(Customer customer, CancellationToken ct = default(CancellationToken))
        {
            int retryCount = 0;

            while (retryCount < 4)
            {
                try
                {
                    IRepository _repository = _repositoryFactory.Invoke();
                    await _repository.CreateCustomer(customer);
                    return customer;
                }
                catch (Exception e)
                {
                    if (e.InnerException is SqlException)
                    {
                        retryCount += 1;
                        if ( retryCount == 3)
                        {
                            throw e;
                        }
                    }
                    _logger.LogError(e, customer.CustomerId.ToString());
                }
            }

            return new Customer();
        }
    }


    public interface ICache
    {
        /// <summary>
        /// Get's an object from the cache. Returns null if the object is not there.
        /// </summary>
        /// <param name="key">the key the object is cached under</param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// Sets the object in the cache.
        /// </summary>
        /// <param name="key">the key the object will be cached under</param>
        /// <param name="valueToCache"></param>
        /// <exception cref="ArgumentException">Thrown when an object with the specified key already exists</exception>
        void Add<T>(string key, T valueToCache);
    }

    /// <summary>
    /// Interface for the store service. This is registered as a singleton in asp.net container
    /// </summary>
    public interface IStoreService
    {
        /// <summary>
        /// Retrieve a list of stores for the provided country code.
        /// </summary>
        /// <param name="countryCode">The code of the country that the store belongs to.</param>
        /// <returns></returns>
        Task<ICollection<Store>> GetStores(string countryCode, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// Creates the customer in the database and returns newly created customer
        /// </summary>
        /// <param name="customer">The customer to save.</param>
        /// <returns>The newly created customer</returns>
        Task<Customer> CreateCustomer(Customer customer, CancellationToken ct = default(CancellationToken));
    }


    /// <summary>
    /// Repository that access the database to retrieve stores or saves customers.
    /// </summary>
    public interface IRepository : IDisposable
    {

        /// <summary>
        /// Retrieves the stores that match the filter expression.
        /// </summary>
        /// <param name="filter">Filter expression.</param>
        /// <returns>stores that match the filter.</returns>
        Task<ICollection<Store>> GetStores(Func<Store, bool> filter, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// Adds a customer to the database.
        /// </summary>
        /// <param name="customer">The customer to add.</param>
        /// <exception cref="SqlException">When the database is very busy, it might throw a SQL Exception that indicates there is a deadlock.</exception>
        /// <returns>The newly created customer</returns>
        Task<Customer> CreateCustomer(Customer customer);
    }

    public class Store
    {
        public int StoreId { get; set; }
        public string CountryCode { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public int StoreId { get; set; }
        public string Email { get; set; }
    }
}