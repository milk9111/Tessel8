using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class ArenaOption : MonoBehaviour
    {
        public GameObject infoPanel;
        public string arenaName;

        private bool _isSelected;
        private Text _text;
        private string _originalText;
        private ArenaSelection _selection;

        void Awake()
        {
            _selection = GetComponentInParent<ArenaSelection>();
            _text = GetComponent<Text>();
            _originalText = _text.text;
            Deselect();
        }

        public void Select()
        {          
            infoPanel.SetActive(true);
            _text.text = _originalText + " <";

            if (_isSelected) return;
            
            _isSelected = true;
            _selection.OnNewSelection(this);
        }

        public void Deselect()
        {
            infoPanel.SetActive(false);
            _text.text = _originalText;
            _isSelected = false;
        }
    }
}