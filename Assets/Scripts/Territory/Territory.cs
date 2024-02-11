using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.Territories
{
    public class Territory
    {
        // TO-DO: TerritoryManager loaded when all territories are loaded

        private MeshRenderer c_mRenderer;

        private readonly MapColorScheme _colorScheme;

        private readonly TerritoryEMB _emb;
        public TerritoryEMB EMB => _emb;

        private readonly TileData _tileData;
        private readonly TerritoryData _territoryData;
        private readonly ITerritoryHandler _handler;

        public Territory(MapColorScheme color_scheme, TerritoryIdentifier identifier, ITerritoryHandler handler)
        {
            _interactable = true;

            _colorScheme = color_scheme;

            _emb = new(this, identifier.gameObject);

            _tileData = identifier.TileData;
            _territoryData = identifier.TerritoryData;

            Cache();

            _handler = handler;

            SetOwner(identifier.Owner);

            handler.OnStateChanged += (state) =>
            {
                if (state.Equals(TerritoryHandlerState.LOADING))
                    GetNeighborTerritories();
            };
        }

        private bool _interactable;
        public bool Interactable => _interactable;
        public void SetInteractable(bool interactable) =>
            _interactable = interactable;

        // ##### OCCUPATION ##### \\

        private ITerritoryOccupant _occupant;
        public ITerritoryOccupant Occupant => _occupant;

        public void Occupy(ITerritoryOccupant occupant)
        {
            if (_occupant != null)
                throw new System.ArgumentException("Can't occupy territory which is already occupied.");
            _occupant = occupant;
            SetOwner(_occupant.Owner);
        }

        public void Deocuppy()
        {
            _occupant = null;
        }

        // ##### SELECTION ##### \\

        private bool _selected;
        public bool Selected => _selected;

        private float _selectionStartTime;

        private Coroutine _selectionCoroutine;

        public enum SelectionType
        {
            ACTIVE,
            HIGHLIGHT
        }

        public void EnableSelection(SelectionType selection_type)
        {
            if (_selected)
                return;
            if (_selectionCoroutine != null)
                _emb.StopCoroutine(_selectionCoroutine);

            if (selection_type.Equals(SelectionType.ACTIVE))
                SetSecondaryColor(_colorScheme.Active);
            else if (selection_type.Equals(SelectionType.HIGHLIGHT))
                SetSecondaryColor(_colorScheme.Highlighted);

            _selected = true;
            _selectionStartTime = Time.time;
            if (_selectionCoroutine != null)
                _emb.StopCoroutine(_selectionCoroutine);
            _selectionCoroutine = _emb.StartCoroutine(SelectionRoutine());
        }

        public void DisableSelection()
        {
            if (!_selected)
                return;
            if (_selectionCoroutine != null)
                _emb.StopCoroutine(_selectionCoroutine);
            _selected = false;
            _selectionStartTime = Time.time;
            if (_selectionCoroutine != null)
                _emb.StopCoroutine(_selectionCoroutine);
            _selectionCoroutine = _emb.StartCoroutine(SelectionRoutine());
        }

        private IEnumerator SelectionRoutine()
        {
            float currentPercentage = c_mRenderer.materials[0].GetFloat("_SelectionPercentage");
            if (_selected)
            {
                float percentageChange = (Time.time - _selectionStartTime) / _tileData.SelectionDuration;
                currentPercentage += percentageChange;
                if (currentPercentage > 1f)
                    currentPercentage = 1f;
                c_mRenderer.materials[0].SetFloat("_SelectionPercentage", currentPercentage);
                yield return new WaitForSeconds(Time.deltaTime);
                if (currentPercentage < 1f)
                    yield return SelectionRoutine();
            }
            else
            {
                float percentageChange = (Time.time - _selectionStartTime) / _tileData.SelectionDuration;
                currentPercentage -= percentageChange;
                if (currentPercentage < 0f)
                    currentPercentage = 0f;
                c_mRenderer.materials[0].SetFloat("_SelectionPercentage", currentPercentage);
                yield return new WaitForSeconds(Time.deltaTime);
                if (currentPercentage > 0f)
                    yield return SelectionRoutine();
            }
        }

        // #### GRAPHICS ##### \\

        private void SetMainColor(Color color)
        {
            c_mRenderer.materials[0].SetColor("_MainColor", color);
        }

        private void SetSecondaryColor(Color color)
        {
            c_mRenderer.materials[0].SetColor("_SecondaryColor", color);
        }

        // ##### OWNERSHIP ##### \\

        private ITerritoryOwner _owner;
        public ITerritoryOwner Owner => _owner;

        public void SetOwner(PlayerIdentification owner_type)
        {
            switch (owner_type)
            {
                case PlayerIdentification.PLAYER:
                    SetOwner(_handler.Player);
                    break;

                case PlayerIdentification.OTHER_PLAYER:
                    SetOwner(_handler.OtherPlayer);
                    break;

                case PlayerIdentification.NEUTRAL:
                    SetOwner(_handler.Neutral);
                    break;
            }
        }

        public void SetOwner(ITerritoryOwner owner)
        {
            _owner = owner;

            switch(owner.Identification)
            {
                case PlayerIdentification.PLAYER:
                    SetMainColor(_colorScheme.PlayerOne);
                    break;

                case PlayerIdentification.OTHER_PLAYER:
                    SetMainColor(_colorScheme.PlayerTwo);
                    break;

                case PlayerIdentification.NEUTRAL:
                    SetMainColor(_colorScheme.NeutralPlayer);
                    break;
            }
        }

        // ##### SURROUNDING TERRITORIES ##### \\

        private Territory[] _neighborTerritories;
        public Territory[] NeighborTerritories => _neighborTerritories;

        private void GetNeighborTerritories()
        {
            List<Territory> neighborTerritories = new();

            int layerMask = LayerMask.GetMask("Territory");

            Collider[] hits;
            hits = Physics.OverlapSphere(_emb.transform.position, _emb.transform.parent.localScale.x * _tileData.TileRadius, layerMask);

            foreach (Collider c in hits)
            {
                TerritoryEMB t = (TerritoryEMB)c.transform.parent.GetComponent<TerritoryEMB.EMB>().Encapsulator;
                if (t.Territory.Equals(this))
                    continue;
                neighborTerritories.Add(t.Territory);
            }
            _neighborTerritories = neighborTerritories.ToArray();
        }

        public bool IsNeighbor(Territory other_territory)
        {
            foreach (Territory t in _neighborTerritories)
            {
                if (t.Equals(other_territory))
                    return true;
            }
            return false;
        }

        // ##### CACHE ##### \\

        private void Cache()
        {
            for (int i = 0; i < _emb.transform.childCount; i++)
            {
                if (_emb.transform.GetChild(i).name.Equals("model"))
                {
                    c_mRenderer = _emb.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>();
                    break;
                }
            }

            if (c_mRenderer.Equals(null))
                throw new UnityException("A Territory game object must have a 'model' (case-sensitive)" +
                    "child for rendering the tile mesh.");
        }

        // ##### MONO BEHAVIOUR ##### \\

        public sealed class TerritoryEMB : EncapsulatedMonoBehaviour
        {
            private Territory _territory;
            public Territory Territory => _territory;

            public TerritoryEMB(Territory territory, GameObject gameObject) : base(gameObject)
            {
                _territory = territory;
            }
        }
    }
}