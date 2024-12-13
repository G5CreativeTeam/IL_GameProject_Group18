using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public TextAsset inkFile;
    public GameObject textBox;
    public GameObject customButton;
    public GameObject optionPanel;
    public GameObject additionalObject;
    public GameObject nextStep;
    public float TextSpeed = 1;

    [HideInInspector]public bool isTalking = false;


    static Story story;
    TextMeshProUGUI nametag;
    TextMeshProUGUI message;
    List<string> tags;
    static Choice choiceSelected;

    // Start is called before the first frame update
 

    private void Update()
    {
        
        if (Input.GetMouseButtonUp(0))
        {
            if (message.text != story.currentText)
            {
                message.text = story.currentText;
                StopAllCoroutines(); 
            } else { 
            //Is there more to the story?
                if (story.canContinue)
                {
                
                    AdvanceDialogue();

                //Are there any choices?
                    if (story.currentChoices.Count != 0)
                    {
                        StartCoroutine(ShowChoices());
                    }
                }
                else
                {
                    FinishDialogue();
                }
            }
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            FinishDialogue(); 
        }

    }

    public void InitiateDialogue()
    {
        if (additionalObject != null) {
            additionalObject.SetActive(true);
        }
        story = new Story(inkFile.text);
        nametag = textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        message = textBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        tags = new List<string>();
        choiceSelected = null;
    }

    // Finished the Story (Dialogue)
    private void FinishDialogue()
    {
        if (nextStep != null)
        {
            nextStep.SetActive(true);
        }
        
    }

    // Advance through the story 
    public void AdvanceDialogue()
    {
        GameObject Character;
        if (textBox.transform.childCount >= 3)
        {
            textBox.transform.GetChild(2).gameObject.SetActive(false);
        }
        
        if (story.currentTags.Count != 0)
        {
            Character = GameObject.Find(story.currentTags[1]);
            Character.GetComponent<Image>().enabled = false;
        }
        
        string currentSentence = story.Continue();
        currentSentence = story.currentText;

        if (story.currentTags.Count != 0)
        {
            nametag.text = story.currentTags[0];

            Character = GameObject.Find(story.currentTags[1]);
            
            Character.GetComponent<Image>().enabled = true;
            
        }
        
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    // Type out the sentence letter by letter and make character idle if they were talking
    IEnumerator TypeSentence(string sentence)
    {
        message.text = "";
        int index = 0;
        while (message.text != sentence)
        {

            message.text += sentence[index];
            index++;

            yield return new WaitForSecondsRealtime(0.1f / TextSpeed);
            
        }
        if (textBox.transform.childCount >= 3)
        {
            textBox.transform.GetChild(2).gameObject.SetActive(true);
        }

    }

    // Create then show the choices on the screen until one got selected
    IEnumerator ShowChoices()
    {
        Debug.Log("There are choices need to be made here!");
        List<Choice> _choices = story.currentChoices;

        for (int i = 0; i < _choices.Count; i++)
        {
            GameObject temp = Instantiate(customButton, optionPanel.transform);
            temp.transform.GetChild(0).GetComponent<Text>().text = _choices[i].text;
            temp.AddComponent<Selectable>();
            temp.GetComponent<Selectable>().element = _choices[i];
            temp.GetComponent<Button>().onClick.AddListener(() => { temp.GetComponent<Selectable>().Decide(); });
        }

        optionPanel.SetActive(true);

        yield return new WaitUntil(() => { return choiceSelected != null; });

        AdvanceFromDecision();
    }

    // Tells the story which branch to go to
    public static void SetDecision(object element)
    {
        choiceSelected = (Choice)element;
        story.ChooseChoiceIndex(choiceSelected.index);
    }

    // After a choice was made, turn off the panel and advance from that choice
    void AdvanceFromDecision()
    {
        optionPanel.SetActive(false);
        for (int i = 0; i < optionPanel.transform.childCount; i++)
        {
            Destroy(optionPanel.transform.GetChild(i).gameObject);
        }
        choiceSelected = null; // Forgot to reset the choiceSelected. Otherwise, it would select an option without player intervention.
        AdvanceDialogue();
    }

    public void SetCharacter(string character)
    {
        GameObject charObject = GameObject.Find(character);
        charObject.SetActive(true);
    }
    /*** Tag Parser ***/
    /// In Inky, you can use tags which can be used to cue stuff in a game.
    /// This is just one way of doing it. Not the only method on how to trigger events. 
    void ParseTags()
    {
        tags = story.currentTags;
        foreach (string t in tags)
        {
            string prefix = t.Split(' ')[0];
            string param = t.Split(' ')[1];

            switch (prefix.ToLower())
            {
                case "anim":
                    break;
                case "color":
                    SetTextColor(param);
                    break;
            }
        }
    }


    void SetTextColor(string _color)
    {
        switch (_color)
        {
            case "red":
                message.color = Color.red;
                break;
            case "blue":
                message.color = Color.cyan;
                break;
            case "green":
                message.color = Color.green;
                break;
            case "white":
                message.color = Color.white;
                break;
            default:
                Debug.Log($"{_color} is not available as a text color");
                break;
        }
    }

}