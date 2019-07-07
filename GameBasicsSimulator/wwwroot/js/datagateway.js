DATAGATEWAY = function () {
    var host = 'https://localhost:5001/';
    var apiUrl = 'api/';

    function loadData(controller) {
        var deferred = $.Deferred();

        $.ajax({
            method: "GET",
            dataType: "json",
            url: host + apiUrl + controller,
            success: function (data, message, xhr) {
                console.log('Successfully retrieved data from server');
                deferred.resolve(data);
            },
            error: function (xhr, err, msg) {
                console.log(msg);
                deferred.reject(msg);
            }
        })

        return deferred.promise();
    }

    function updateData(controller, data) {
        var deferred = $.Deferred();

        $.ajax({
            method: "POST",
            url: host + apiUrl + controller,
            contentType: "application/json",
            dataType: "json",
            data: data,
            success: function (data, message, xhr) {
                console.log('Successfully executed update');
                deferred.resolve(data);
            },
            error: function (xhr, err, msg) {
                console.log(msg);
                deferred.reject(msg);
            }
        });

        return deferred.promise();
    }

    return {
        load: loadData,
        update: updateData
    }
}();

