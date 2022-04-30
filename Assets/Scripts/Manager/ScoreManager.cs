using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    //variáveis
    private string TopScoresURL;
    private string username;
    private int _highscore;
    private int _lowestHigh;
    private bool _scoresRead;
    private bool _isTableFound;

    
    public class Score
    {
        public string name { get; set; }
        public int score { get; set; }

        public Score(string n, int s)
        {
            name = n;
            score = s;
        }

        public Score(string n, string s)
        {
            name = n;
            score = Int32.Parse(s);
        }
    }

    List<Score> scoreList = new List<Score>(10); //top scores

    void OnLevelWasLoaded(int level)
    {
        if (level == 2) StartCoroutine("UpdateGUIText");    // se o score for carregado
        if (level == 1) _lowestHigh = _highscore = 99999;
    }

    IEnumerator GetHighestScore() //parte interna dos scores / bd
    {
        // esperar até os scores serem puxados da base de dados
        float timeOut = Time.time + 4;
        while (!_scoresRead)
        {
            yield return new WaitForSeconds(0.01f);
            if (Time.time > timeOut)
            {
                break;
            }
        }

        _highscore = scoreList[0].score; //para o high score
        _lowestHigh = scoreList[scoreList.Count - 1].score; //mesmo que não seja high score ainda pode ficar nos top scores
    }

    IEnumerator UpdateGUIText() //parte do UI dos scores
    {
        
        // esperar como antes
        float timeOut = Time.time + 4;
        while (!_scoresRead)
        {
            yield return new WaitForSeconds(0.01f);
            if (Time.time > timeOut)
            {   
                scoreList.Clear();
                scoreList.Add(new Score("DATABASE TEMPORARILY UNAVAILABLE", 999999));
                break;
            }
        }
        scoreList.Clear();
        scoreList.Add(new Score("DATABASE TEMPORARILY UNAVAILABLE", 999999));
        yield return new WaitForSeconds(0f);
    }

    IEnumerator ReadScoresFromDB() //vai ler os scores da BD
    {
        WWW GetScoresAttempt = new WWW(TopScoresURL);
        yield return GetScoresAttempt;

        if (GetScoresAttempt.error != null)
        {
            scoreList.Add(new Score(GetScoresAttempt.error, 1234));
            StartCoroutine(UpdateGUIText());
        }
        else
        {
            //isto vai assumir que a query vai encontrar a tabela
            string[] textlist = GetScoresAttempt.text.Split(new string[] { "\n", "\t" },
                StringSplitOptions.RemoveEmptyEntries);

            if (textlist.Length == 1)
            {
                scoreList.Clear();
                scoreList.Add(new Score(textlist[0], -123));
                yield return null;
            }
            else
            {
                string[] Names = new string[Mathf.FloorToInt(textlist.Length/2)];
                string[] Scores = new string[Names.Length];

                for (int i = 0; i < textlist.Length; i++)
                {
                    if (i%2 == 0)
                    {
                        Names[Mathf.FloorToInt(i/2)] = textlist[i];
                    }
                    else Scores[Mathf.FloorToInt(i/2)] = textlist[i];
                }

                for (int i = 0; i < Names.Length; i++)
                {
                    scoreList.Add(new Score(Names[i], Scores[i]));
                }

                _scoresRead = true;
            }
        }

    }

    public int High()
    {
        return _highscore;
    }

    public int LowestHigh()
    {
        return _lowestHigh;
    }
}
