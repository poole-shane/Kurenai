using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameCard : MonoBehaviour
{
    [System.Serializable]
    public class EntityParams
    {
        public int Index;
        public CardType Type;
    }

    private bool _selected;
    private Material _textureMaterial;
    public EntityParams Entity;

    public PlayerMain Player;
    public GameObject CardObject;
    public List<Texture> Textures = new List<Texture>(Global.CARD_TYPES_MAX);

    public bool Selected => _selected;

    public void SetEntity(EntityParams entity)
    {
        Entity = entity;
        _textureMaterial = CardObject.GetComponent<MeshRenderer>().materials[2];

        _textureMaterial.SetTexture("_CardFront", Textures[(int)this.Entity.Type]);
    }

    // Start is called before the first frame update
    void Start()
    {
        EntityParams test = new EntityParams();
        test.Index = UnityEngine.Random.Range(0, 8);
        test.Type = (CardType)UnityEngine.Random.Range(0, Global.CARD_TYPES_MAX);

        SetEntity(test);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseUp()
    {
        UpdateSelection();
    }

    public void UpdateSelection()
    {
        _selected = !_selected;
        CardObject.transform.localRotation = _selected ? Quaternion.Euler(-90, 0, 0) : Quaternion.Euler(-90, 180, 0);

        Player?.CardFlipped(this.Entity);
    }

}
