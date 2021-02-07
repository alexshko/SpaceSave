using AirBattle.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AirBattle.UI.Menu
{


    public class MenuUI : MonoBehaviour
    {
        public List<Button> OpenMenuMappingKey;
        public List<GameObject> OpenMenuMappingValue;
        public Slider TargetsSlider;

        private SortedList<int, Button> ButtonGroupings;

        private void Start()
        {
            if (OpenMenuMappingKey.Count != OpenMenuMappingValue.Count)
            {
                Debug.LogError("There is an error with the mapping");
            }
            ButtonGroupings = new SortedList<int, Button>();
        }

        public void openMenu()
        {
            //deactivate all other panels:
            foreach (var panel in OpenMenuMappingValue)
            {
                panel.SetActive(false);
            }

            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (button != null)
            {
                //if the button is in the list of mappings, then open the Menu mapped to it.
                if (OpenMenuMappingKey.Contains(button))
                {
                    int indx = OpenMenuMappingKey.IndexOf(button);
                    GameObject mappedPanel = OpenMenuMappingValue[indx];
                    mappedPanel.SetActive(true);
                }

                UpdateLastButtonInGroup();
            }
        }

        public void UpdateLastButtonInGroup()
        {

            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (button != null)
            {
                //add the button to the list of pressed buttons where indx is the group. change the prev button:
                MenuUIButtonGroup gr;
                if (button.TryGetComponent<MenuUIButtonGroup>(out gr))
                {
                    //first if there is previous button in the group then we should make it interactable
                    if (ButtonGroupings.ContainsKey(gr.group))
                    {
                        ButtonGroupings[gr.group].interactable = true;
                    }
                    ButtonGroupings[gr.group] = button;
                }

                //make sure all the groups activated buttons are indeed activated:
                if (ButtonGroupings.Count > 0)
                {
                    foreach (var btn in ButtonGroupings.Values)
                    {
                        btn.interactable = false;
                    }
                }
            }
        }

        //public void OpenMenuTrain()
        //{
        //    Debug.Log("Open Main Menu");
        //}
        public void MenuExit()
        {
            Debug.Log("Exit");
            Application.Quit(0);
        }

        public void LoadTrainSpreeKill()
        {
            SinglePlayerGameManagement.GameType = "KillSpree";
            SinglePlayerGameManagement.NumOfTargets = (int)TargetsSlider.value;
            StartCoroutine(LoadSceneForTrain());
        }


        public IEnumerator LoadSceneForTrain()
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync("Scenes/GameScene");
            while (!ao.isDone)
            {
                yield return null;
            }
        }
    }
}
