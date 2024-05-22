using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GameCard : MonoBehaviour
{
    private enum EventType { Flip, Disappear }
    private enum FlipDirection { ToFront, ToBack }

    [System.Serializable]
    public class EntityParams
    {
        public int Index;
        public CardType Type;
        public bool Solved;
    }

    private EventType _eventFlag;
    private float _eventElapsed = 0, _flipElapsed;
    private FlipDirection _flipDirection;
    private bool _selected = false;
    private Vector3 _rotation;
    private Sprite _frontSprite;

    public EntityParams Entity;
    public PlayerMain Player;
    public AudioSource CardFlipSound;
    public Image CardFace;
    public Sprite BackSprite;
    public List<Sprite> CardSprites = new List<Sprite>(GlobalConstants.CARD_TYPES_MAX);

    /*
     * Parameters that can be tweaked in inspector
     */
    public float _eventDelay = .5f;
    public float _flipDuration = .16f;

    public bool Selected => _selected;

    public void SetEntity(EntityParams entity)
    {
        Entity = entity;

        CardFace.sprite = BackSprite;
        _frontSprite = CardSprites[(int)this.Entity.Type];

        if (Entity.Solved)
            DisappearCard();
        else
        {
            // Add slight tilt to cards
            _rotation = new Vector3(0, 180, Random.Range(-3, 3));
            UpdateRotation();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _eventElapsed = _eventDelay;
        _flipElapsed = _flipDuration;
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

        if(_flipElapsed < _flipDuration)
        {
            _flipElapsed += Time.deltaTime;

            float oldRot = _rotation.y;

            switch (_flipDirection)
            {
                case FlipDirection.ToFront:
                    _rotation.y = Mathf.Lerp(180, 0, _flipElapsed / _flipDuration);
                    if (oldRot > 90 && _rotation.y <= 90)
                        CardFace.sprite = _frontSprite;

                    if (_flipElapsed >= _flipDuration)
                        _rotation.y = 0;
                    UpdateRotation();
                    break;
                case FlipDirection.ToBack:
                    _rotation.y = Mathf.Lerp(0, 180, _flipElapsed / _flipDuration);
                    if (oldRot < 90 && _rotation.y >= 90)
                        CardFace.sprite = BackSprite;

                    if (_flipElapsed >= _flipDuration)
                        _rotation.y = 180;
                    UpdateRotation();
                    break;
            }
        }
    }

    private void OnMouseUp()
    {
        if (_eventElapsed >= _eventDelay && !_selected)
            UpdateSelection();
    }

    private void UpdateRotation()
    {
        transform.localRotation = Quaternion.Euler(_rotation);
    }

    public void UpdateSelection()
    {
        FlipCard();
        Player?.CardTapped(this.Entity);
    }

    private void FlipCard()
    {
        _selected = !_selected;

        _flipDirection = _selected ? FlipDirection.ToFront : FlipDirection.ToBack;
        _flipElapsed = 0;

        // Set rotation to starting value
        _rotation.y = _selected ? 180 : 0;
        UpdateRotation();

        // Play sound if the card's front side has been revealed
        if (_selected)
            CardFlipSound.Play();
    }

    private void DisappearCard()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Flips the card back. Called when a mismatch occurs and the
    /// card reverts to its original unflipped state.
    /// </summary>
    public void FlipToBack()
    {
        _eventElapsed = 0;
        _eventFlag = EventType.Flip;
    }

    /// <summary>
    /// Call to set the card to the solved state and make card disappear.
    /// </summary>
    public void Solve()
    {
        Entity.Solved = true;
        _eventElapsed = 0;
        _eventFlag = EventType.Disappear;
    }
}
