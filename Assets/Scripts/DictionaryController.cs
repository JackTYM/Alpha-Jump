using System.Collections;
using System.Collections.Generic; // for using HashSet
using UnityEngine;

public class DictionaryController : MonoBehaviour
{
    public TextAsset wordFile; // the text asset file which contains the words
    public int modeIndex = 0; // mode index for different game modes
    public HashSet<string> dictionary = new HashSet<string>(); // HashSet to store the words

    // Start is called before the first frame update
    void Start()
    {
        dictionary = new HashSet<string>(wordFile.text.Split(","[0])); // split the text file by "," and store the words in the HashSet
    }

    public bool validWord(string word) { // function to check if a word is valid
        return dictionary.Contains(word); // check if the word is in the HashSet
    }
}