using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldAnimationManager : MonoBehaviour
{
    //static AnimationManager instance = null;

    //class AnimationInfo
    //{
    //    public int id = 0;
    //    public Character character = Character.MAGNETO;
    //    public State state = State.IDLE;
    //    public State previousState = State.IDLE;
    //    public int animationCount = 0;
    //}

    //public enum Character { MAGNETO };

    //public enum State { IDLE };

    //public Sprite[] spriteBank;

    //Dictionary<Character, string> charAnimDic = new Dictionary<Character, string>();
    //Dictionary<State, string> stateAnimDic = new Dictionary<State, string>();
    //Dictionary<GameObject, AnimationInfo> objectDic = new Dictionary<GameObject, AnimationInfo>();
    //int numberOfObjects = 0;

    //List<GameObject> activeObjects = new List<GameObject>();
    //List<AnimationInfo> activeObjectsInfo = new List<AnimationInfo>();

    //private void Awake()
    //{
    //    //instance = this;


    //    charAnimDic.Add(Character.MAGNETO, "Magneto");
    //    stateAnimDic.Add(State.IDLE, "Idle");

    //}

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    //public void GetNextAnimation(GameObject go)
    //{
    //    if (!go.GetComponent<SpriteRenderer>())
    //        return;

    //    if (objectDic.ContainsKey(go))
    //    {
    //        /// var animInfo = objectDic[go];
    //        objectDic[go].previousState = objectDic[go].state;
    //      //  objectDic[go].state = go.GetComponent<PlayerAnimation>().state;
    //        if (objectDic[go].state == objectDic[go].previousState && objectDic[go].animationCount < spriteBank.Length)
    //            objectDic[go].animationCount++;
    //        else
    //            objectDic[go].animationCount = 0;

    //        var val = GetNextAnimationNumber(objectDic[go]);
    //        go.GetComponent<SpriteRenderer>().sprite = spriteBank[val];
    //    }
    //    else
    //    {
    //        AnimationInfo animInfo = new AnimationInfo();
    //       // animInfo.character = go.GetComponent<PlayerAnimation>().character;
    //       // animInfo.state = go.GetComponent<PlayerAnimation>().state;
    //        //animInfo.previousState = go.GetComponent<PlayerAnimation>().state;
    //        animInfo.id = numberOfObjects;
    //        numberOfObjects++;
    //        animInfo.animationCount = 0;
    //        objectDic.Add(go, animInfo);
    //        var val = GetNextAnimationNumber(animInfo);
    //        go.GetComponent<SpriteRenderer>().sprite = spriteBank[val];
    //    }

    //    //if(activeObjects.Contains(go))
    //    //{
    //    //    activeObjects.IndexOf(go);
    //    //}

    //}

    //int GetNextAnimationNumber(AnimationInfo animInfo)
    //{

    //    var charName = charAnimDic[animInfo.character];
    //    var charState = stateAnimDic[animInfo.state];
    //    var animNum = animInfo.animationCount;
    //    //if (animInfo.animationCount >= spriteBank.Length)
    //    //    animInfo.animationCount = 0;
    //    for (int i = 0; i < spriteBank.Length; ++i)
    //    {
    //        var spriteName = charName + "_" + charState + "_" + animInfo.animationCount.ToString();
    //        if (spriteBank[i].name == spriteName)
    //            return i;
    //    }
    //    return 0;
    //}

    //public static AnimationManager Instance() { return instance; }
}
