var TEAMS = [];
var CURRENT_MATCH;

$(function () {
    function initTeams(teams) {
        TEAMS = teams.sort(function (a, b) {
            return b.points - a.points;
        });   
        setStandings();

        var matches = createMatches();
        setMatches(matches);
    }

    $.when(DATAGATEWAY.load('team')).done(initTeams);
});

function setStandings() {
    var goals, goalsconceded, diff;
    for (var i = 0; i < TEAMS.length; i++) {
        goals = (TEAMS[i].goals ? TEAMS[i].goals.length : 0);
        goalsconceded = (TEAMS[i].goalsConceded ? TEAMS[i].goalsConceded.length : 0);
        diff = goals - goalsconceded;

        $(".teamName")[i].innerHTML = TEAMS[i].name;
        $(".played")[i].innerHTML = (TEAMS[i].matchTeams ? TEAMS[i].matchTeams.length : 0); 
        $(".goalsFor")[i].innerHTML = goals;
        $(".goalsAgainst")[i].innerHTML = goalsconceded;
        $(".goalsDiff")[i].innerHTML = diff;
        $(".points")[i].innerHTML = TEAMS[i].points;
    }
}

function createMatches() {
    function findIfNotUsedTeam(val, arr) {
        var tot = [];
        for(var i = 0; i < arr.length; i++){
            tot = tot.concat(arr[i]);  
        }
        return !tot.includes(val);
    }

    function findIfNotUsedMatch(match, arr) {
        var tot = [];
        for(var i = 0; i < arr.length; i++){
            tot = tot.concat(arr[i]);  
        }
        
        for (var i = 0; i < tot.length; i++ ) {
            if (tot[i].includes(match[0]) && tot[i].includes(match[1])) {
                return false;
            }
        }
        return true;
    }

    var res = [], match;
    for (var i = 0; i < TEAMS.length-1; i++) {
        res.push([]);   
        for (var j = 0; j < TEAMS.length; j++) {
            if (findIfNotUsedTeam(TEAMS[j], res[i])) {          
                for (var k = 0; k < TEAMS.length; k++) {
                    if (TEAMS[j] !== TEAMS[k] && findIfNotUsedTeam(TEAMS[k], res[i])) {
                        match = [TEAMS[j], TEAMS[k]];
                        if (findIfNotUsedMatch(match, res)) {
                            res[i].push([TEAMS[j], TEAMS[k]]);    
                            break;
                        }
                    }
                }     
            }
        }   
    }

    return res;
}

function setMatches(matches) {
    var allMatches = [];
    for (var i = 0; i < matches.length; i++) {
        allMatches = allMatches.concat(matches[i]);
    }

    var matchDivs = $("div.match");
    if (matchDivs.length === allMatches.length) {
        for (var i = 0; i < matchDivs.length; i++) {
            $(matchDivs[i]).find("span")[0].innerHTML = allMatches[i][0].name;
            $(matchDivs[i]).find("span")[1].innerHTML = allMatches[i][1].name;
            $(matchDivs[i]).find("input").click(function (e) {
                $(e.currentTarget).hide();
                $(e.currentTarget.parentElement).next().find("input").show();
                CURRENT_MATCH = allMatches[parseInt(e.currentTarget.parentElement.id)];
                startMatch(CURRENT_MATCH);
            });
        }
    }
}

function startMatch(matchTeams) {
    function onSuccess(data) {
        setMatchScore(data.score);
    }

    $.when(DATAGATEWAY.update('match', JSON.stringify({ Teams: [matchTeams[0], matchTeams[1]] }))).done(onSuccess);
}

function setMatchScore(score) {
    $("#teams-wrapper").html("<span>" + CURRENT_MATCH[0].name + "</span> - " +
                            "<span>" + CURRENT_MATCH[1].name + "</span>")
    $("#score-wrapper").html("<h2>" + score + "</h2>");

    $.when(DATAGATEWAY.load('team')).done(updateStanding);
}

function updateStanding(teams) {
    TEAMS = teams.sort(function (a, b) {
        return b.points - a.points;
    });   

    setStandings();

    CURRENT_MATCH = null;
}