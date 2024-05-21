using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameCard : MonoBehaviour
{
    private const int CHANGE_MATERIAL_NO = 1;
    private enum EventType { Flip, Disappear }

    [System.Serializable]
    public class EntityParams
    {
        public int Index;
        public CardType Type;
    }

    private EventType _eventFlag;
    private float _eventDelay = .5f, _eventElapsed = 0;
    private bool _selected = false;
    private Material _textureMaterial;
    public EntityParams Entity;

    public PlayerMain Player;
    public GameObject CardObject;
    public AudioSource CardFlipSound;
    public List<Texture> Textures = new List<Texture>(Global.CARD_TYPES_MAX);

    public bool Selected => _selected;

    public void SetEntity(EntityParams entity)
    {
        Entity = entity;
        _textureMaterial = CardObject.GetComponent<MeshRenderer>().materials[CHANGE_MATERIAL_NO];

        _textureMaterial.SetTexture("_CardFront", Textures[(int)this.Entity.Type]);
    }

    // Start is called before the first frame update
    void Start()
    {
        /*EntityParams test = new EntityParams();
        test.Index = UnityEngine.Random.Range(0, 8);
        test.Type = (CardType)UnityEngine.Random.Range(0, Global.CARD_TYPES_MAX);

        SetEntity(test);*/
        _eventElapsed = _eventDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (_eventElapsed < _eventDelay)
        {
            _eventElapsed += Time.deltaTime;

            if(_eventElapsed >= _eventDelay)
            {
                switch(_eventFlag)
                {
                    case EventType.Flip:
                        FlipCard();
                        break;
                    case EventType.Disappear:
                        DisappearCard();
                        break;

                }
            }
        }
    }

    private void OnMouseUp()
    {
        if (_eventElapsed >= _eventDelay && !_selected)
            UpdateSelection();
    }

    public void UpdateSelection()
    {
        FlipCard();
        Player?.CardTapped(this.Entity);
    }

    private void FlipCard()
    {
        _selected = !_selected;
        CardObject.transform.localRotation = _selected ? Quaternion.Euler(-90, 0, 0) : Quaternion.Euler(-90, 180, 0);

        // Play sound if the card's front side has been revealed
        if (_selected)
            CardFlipSound.Play();
    }

    private void DisappearCard()
    {
        CardObject.SetActive(false);
    }

    public void DelayFlip()
    {
        _eventElapsed = 0;
        _eventFlag = EventType.Flip;
    }

    public void DelayDisappear()
    {
        _eventElapsed = 0;
        _eventFlag = EventType.Disappear;
    }
}
