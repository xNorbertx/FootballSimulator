//GLOBAL VARIABLES
var TEAMS = [];
var CURRENT_MATCH;
var CURRENT_MATCH_IDX;

//Startup function
$(function () {
    function initTeams(teams) {
        setStandings(teams);

        var matches = createMatches();
        setMatches(matches);
    }

    $("#btn-clear").click(clear);

    $.when(DATAGATEWAY.load('team')).done(initTeams);
});

//Set standings for each team
function setStandings(teams) {
    TEAMS = teams.sort(function (a, b) {
        return b.points - a.points;
    });  

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

//Create the 6 different matches that need to be played
function createMatches() {
    var match = [],
    matchDay = [],
    allMatches = [],
    tmpArray,
    tmp,
    max = TEAMS[TEAMS.length - 1],
    current = TEAMS[0],
    len = TEAMS.length;
    for (var i = 1; i<len; i++) {
        tmpArray = [];
        for (var j = 0; j<len; j++) {
            //First match round
            if (i === 1) {
                if (j <= 1) {
                    if (j === 0) {
                        match.push(current);
                        tmpArray.push(TEAMS.shift());
                    } else {
                        match.push(max);
                        tmpArray.push(TEAMS.pop());
                        matchDay.push(match);
                        match = [];
                    } 
                } else {
                    if (j % 2 === 0) {
                        tmp = TEAMS.shift();
                        match.push(tmp);
                        tmpArray.push(tmp);
                    } else {
                        tmp = TEAMS.pop();
                        match.push(tmp);
                        tmpArray.push(tmp);
                        matchDay.push(match);
                        match = [];
                    }
                }
            //Consequent match
            } else {
                if (j <= 1) {
                    if (i % 2 === 1) {
                        if (j === 0) {
                            match.push(max);
                            TEAMS = TEAMS.filter(t => t !== max);
                            tmpArray.push(max);
                        } else {
                            match.push(current);
                            tmpArray.push(TEAMS.pop());
                        }
                    } else {
                        if (j === 0) {
                            match.push(current);
                            tmpArray.push(TEAMS.pop());
                        } else {
                            match.push(max);
                            TEAMS = TEAMS.filter(t => t !== max);
                            tmpArray.push(max);
                        }
                    } 
                    if (j === 1) {
                        matchDay.push(match);
                        match = [];
                    }
                } else {
                    if (j % 2 === 0) {
                        tmp = TEAMS[TEAMS.length - 2];
                        TEAMS = TEAMS.filter(t => t !== tmp);
                        match.push(tmp);
                        tmpArray.push(tmp);
                    } else {
                        tmp = TEAMS[TEAMS.length - 1];
                        TEAMS = TEAMS.filter(t => t !== tmp);
                        match.push(tmp);
                        tmpArray.push(tmp);
                        matchDay.push(match);
                        match = [];
                    }       
                }       
            }
        }
        current = tmpArray[tmpArray.length - 1];
        TEAMS = tmpArray;
        allMatches.push(matchDay);
        matchDay = [];
    }
    return allMatches;
}

//Display the matches from createMatches() on the screen
function setMatches(matches) {
    var allMatches = [];
    for (var i = 0; i < matches.length; i++) {
        allMatches = allMatches.concat(matches[i]);
    }

    var $matchDivs = $("div.match");
    if ($matchDivs.length === allMatches.length) {
        for (var i = 0; i < $matchDivs.length; i++) {
            $($matchDivs[i]).find("span")[0].innerHTML = allMatches[i][0].name;
            $($matchDivs[i]).find("span")[1].innerHTML = allMatches[i][1].name;
            //Set onclick function for the play button
            $($matchDivs[i]).find("input").click(function (e) {
                CURRENT_MATCH_IDX = parseInt(e.currentTarget.parentElement.id);
                CURRENT_MATCH = allMatches[CURRENT_MATCH_IDX];
                startMatch(CURRENT_MATCH);
            });
        }
    }
}

//Start a match
function startMatch(matchTeams) {
    function onSuccess(data) {
        setMatchScore(data.score);
    }

    $.when(DATAGATEWAY.update('match', JSON.stringify({ Teams: [matchTeams[0], matchTeams[1]] }))).done(onSuccess);
}

//Set score of match
function setMatchScore(score) {
    $("#teams-wrapper").html("<span>" + CURRENT_MATCH[0].name + "</span> - " +
                             "<span>" + CURRENT_MATCH[1].name + "</span>")
    $("#score-wrapper").html("<h2>" + score + "</h2>");

    $(".match-result")[CURRENT_MATCH_IDX].innerHTML = score;

    var $activeBtn = $("input.btn-play:visible");
    $activeBtn.hide();
    $activeBtn.parent().next().find("input.btn-play").show();

    $.when(DATAGATEWAY.load('team')).done(setMatchScoreSuccess);
}

//Set new standings and empty global variable
function setMatchScoreSuccess(teams) {
    setStandings(teams);
    CURRENT_MATCH = null;
}

//Handle clear button action
function clear() {
    function onSuccess() {
        $.when(DATAGATEWAY.load('team')).done(clearedSuccessActions);
    }

    $.when(DATAGATEWAY.update('match/clear')).done(onSuccess);
}

//Set new standings and clear old scores from screen
function clearedSuccessActions(teams) {
    var $activeBtn = $("input.btn-play:visible");
    if ($activeBtn.length === 1) {
        $activeBtn.hide();
    }
    $($("div.match")[0]).find(".btn-play").show();

    $(".match-result").html("");
    $("#teams-wrapper").html("");
    $("#score-wrapper").html("");

    setStandings(teams);
}