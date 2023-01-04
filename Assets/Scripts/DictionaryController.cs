using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryController : MonoBehaviour
{

    public TextAsset wordFile;
    public int modeIndex = 0;
    public HashSet<string> dictionary = new HashSet<string>();

    // Start is called before the first frame update
    void Start()
    {
        dictionary = new HashSet<string>(wordFile.text.Split(","[0]));
    }

    public bool validWord(string word) {
        return dictionary.Contains(word);
    }
}
