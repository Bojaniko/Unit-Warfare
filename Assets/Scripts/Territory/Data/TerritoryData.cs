using UnityEngine;

namespace UnitWarfare.Territories
{
    [System.Serializable]
    public class TerritoryData
    {
        [SerializeField] private string _name = "Territory";
        public string Name => _name;

        [SerializeField] private Material _material;
        public Material Material => _material;

        public Texture2D MainImage
        {
            get
            {
                Texture t = _material.GetTexture("_MainTexture");
                Texture2D dest = new Texture2D(t.width, t.height, TextureFormat.RGBA32, true);
                Graphics.CopyTexture(t, dest);
                return dest;
            }
        }
    }
}