using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public Text targetReached;
    public static Score instance;
    public PhotonView pv;

    [SerializeField] 
    GameObject targetReachedHolder;
    public Image textBackground;

    [SerializeField] 
    GameObject scoreAddedHolder;
    Text scoreAddedText;
    RectTransform scoreAddedPos;

    float time = 3f;
    float fadeOutTime = 3f;
    int score = 0;
    int streakFlag = 0;

    private float colorStep = 0;
    private bool fade = false;

    private void Awake()
    {
        instance = this;
        pv = GetComponent<PhotonView>();
        textBackground = targetReachedHolder.GetComponent<Image>();
        scoreAddedText = scoreAddedHolder.GetComponent<Text>();
        scoreAddedPos = scoreAddedHolder.GetComponent<RectTransform>();
    }

    public int GetScore()
    {
        return score;
    }

    void Start()
    {
        scoreText.text = "Score: " + score.ToString();
        scoreAddedText.text = " ";
    }

    public enum HIT : byte
    {
        GOOD, BAD, MINIBOSS, BAD_NOSTREAK
    }

    public void AddScore(HIT type, float fac, bool fireRPC)
    {
        if (fireRPC)
            pv.RPC("AddScoreInternal", RpcTarget.All, (byte)type, fac);
        else
            AddScoreInternal((byte)type, fac);
    }

    [PunRPC]
    private void AddScoreInternal(byte t, float fac)
    {
        HIT type = (HIT)t;
        switch (type)
        {
            case HIT.GOOD:
                score = UpdateScoreValueGoodPerson(score);
                streakFlag = 0;
                scoreAddedText.text = " - 5";
                scoreAddedText.color = new Color32(227, 45, 62, 255);
                Invoke("Hide", time);
                break;
            case HIT.BAD_NOSTREAK:
                score += 10;
                break;
            case HIT.BAD:
                score += (int)(Mathf.Lerp(10f, 50f, fac));             
                streakFlag++;
                scoreAddedText.text = " + 10";
                scoreAddedText.color = new Color32(119, 215, 40, 255);
                Invoke("Hide", time);
                break;
            case HIT.MINIBOSS:
                score = UpdateScoreValueMiniBoss(score);
                streakFlag++;
                targetReached.text = "MISSION COMPLETE";
                textBackground.enabled = true;
                Invoke("Hide", time);
                break;
        }
        scoreText.text = $"Score: {score}";

        if (
            streakFlag > 0 && (
            (streakFlag <= 20 && (streakFlag % 5) == 0)
            || streakFlag % 10 == 0)
        ) {
            targetReached.text = $"{streakFlag} HIT STREAK";
            Invoke("Hide", time);
            textBackground.enabled = true;
        }
    }

    public static int UpdateScoreValueGoodPerson(int scoreToUpdate)
    {
        // Hitting a good person results in a decrease in points
        return scoreToUpdate - 5;
    }

    public static int UpdateScoreValueBadPerson(int scoreToUpdate)
    {
        // Hitting a bad person results in an increase in points
        return scoreToUpdate + 10;
    }

    public static int UpdateScoreValueMiniBoss(int scoreToUpdate)
    {
        // Defeating a miniboss results in an extra large increase in points
        return scoreToUpdate + 100;
    }

    void Hide()
    {
        FadeOutRoutine(targetReached);
        targetReached.text = "";
        textBackground.enabled = false;
        fade = true;
        // scoreAddedText.text = "";
    }

    void Update()
    {
        if(fade) 
        {
            Color32 original = scoreAddedText.color;
            scoreAddedText.color = Color.Lerp(original, new Color32(original[0], original[1], original[2], 0), colorStep);
            colorStep += Time.deltaTime/6f;
            StartCoroutine(ExecuteAfterTime());
        }
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(0.5f);  //gives it enough time to fade
        fade = false;
    }

    private IEnumerator FadeOutRoutine(Text text)
    { 
        Color originalColor = text.color;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime) {
            text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t/fadeOutTime));
            Debug.Log("fading");
            yield return null;
        }
    }
}
