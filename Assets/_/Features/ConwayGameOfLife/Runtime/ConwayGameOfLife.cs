using System;
using TMPro;
using UnityEngine;

namespace ConwayGameOfLife.Runtime
{
    public class ConwayGameOfLife : MonoBehaviour
    {
        #region Publics

        #endregion


        #region Unity API

        private void Awake() {
            _propertyBlockDead  = new MaterialPropertyBlock();
            _propertyBlockAlive = new MaterialPropertyBlock();
            _propertyBlockDead.SetColor("_Color", cellDeadColor);
            _propertyBlockAlive.SetColor("_Color", cellAliveColor);
            _cellPopulationList = new GameObject[_gridDimensions.x,_gridDimensions.y];
            _cellPopulationNextState = new bool[_gridDimensions.x,_gridDimensions.y];
            GenerateGRidCell2D();
            _currentTimer = _delayGeneration;
         }

        private void Update()
        {
            if (_toggleAutoMode)
            {
                _currentTimer -= Time.deltaTime;
                if ( _currentTimer < 0)
                {
                    NewCellGeneration();
                    _currentTimer = _delayGeneration;
                    
                }     
            }
        }
        #endregion

        #region Main methods

        private void GenerateGRidCell1D()
        {
            int gridCellCount = _gridDimensions.x * _gridDimensions.y;
            for (int i = 0; i< gridCellCount; i++)
            {
                var cellPos = GetCellPosition(i);
                var cellObject = Instantiate(_cell, cellPos, Quaternion.identity, transform);
                cellObject.name = $"Cell({cellPos})";
            }
        }
        private void GenerateGRidCell2D()
        {
            int gridCellCount = _gridDimensions.x * _gridDimensions.y;
            for (int i = 0; i < _gridDimensions.x; i++)
            {
                for (int j = 0; j < _gridDimensions.y; j++) 
                {                 
                    var cellObject = Instantiate(_cell, new Vector2(i*_prefabScale,j*_prefabScale), Quaternion.identity, transform);
                    cellObject.name = $"Cell({i},{j})";
                    _cellPopulationList[i,j] = cellObject;
                    if (UnityEngine.Random.value < _probabilityOfLifeAtInit)
                    {
                        cellObject.GetComponent<SpriteRenderer>().SetPropertyBlock(_propertyBlockAlive);
                        _cellPopulationNextState[i, j] = true;
                        _cellPopulation++;
                    }
                    else
                    {
                        cellObject.GetComponent<SpriteRenderer>().SetPropertyBlock(_propertyBlockDead);
                        _cellPopulationNextState[i, j] = false;
                    }
                }
            }
        }
        private Vector3 GetCellPosition(int i)
        {
            int x = i % _gridDimensions.x;
            int y = i / _gridDimensions.x;
            return new Vector3(x, y, 0);
        }

        public void NewCellGeneration()
        {
            _cellPopulation = 0;
            for (int i = 0; i < _gridDimensions.x; i++)
            {
                for (int j = 0; j < _gridDimensions.y; j++)
                {
                    var aliveCellAround = GetLivingAround(i, j);
                    if (aliveCellAround == 3) _cellPopulationNextState[i, j] = true;
                    if (aliveCellAround == 2) _cellPopulationNextState[i, j] = _cellPopulationNextState[i, j];
                    if (aliveCellAround <= 1 || aliveCellAround >= 4) _cellPopulationNextState[i, j] = false;                   
                }
            }
            _cellGeneration++;
            RenderNewGeneration();
            _aliveCellCountUI.text = $"Cellules en vie : {_cellPopulation}";
            _genereationCountUI.text = $"Generation : {_cellGeneration}";
        }

        private void RenderNewGeneration()
        {
            for (int i = 0; i < _gridDimensions.x; i++)
            {
                for (int j = 0; j < _gridDimensions.y; j++)
                {
                    if(_cellPopulationNextState[i, j] == true)
                    {
                        _cellPopulationList[i, j].GetComponent<SpriteRenderer>().SetPropertyBlock(_propertyBlockAlive);
                        _cellPopulation++;
                    }
                    else
                    {
                        _cellPopulationList[i, j].GetComponent<SpriteRenderer>().SetPropertyBlock(_propertyBlockDead);
                    }
                }
            }
        }

        private int GetLivingAround(int x, int y)
        {
            int aliveCount = 0;
            for (int i = x-1;  i < x+2; i++) { 
                for(int j = y-1;j< y+2; j++)
                {
                    if (i == x && j == y ) continue;
                    if (GetIndexFrom(new Vector2(i, j)) < 0 || GetIndexFrom(new Vector2(i, j)) >= _gridDimensions.x * _gridDimensions.y) continue;
                    if (GetStateColor(_cellPopulationList[i, j]) == cellAliveColor) aliveCount++;
                }             
            }
            return aliveCount;
        }

        public void ToggleAutoMode() => _toggleAutoMode = !_toggleAutoMode;

        public void QuitApplication() => Application.Quit();

        #endregion

        #region Utils


        // Merci ChatGPT :D
        private Color GetStateColor(GameObject cell)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            cell.GetComponent<SpriteRenderer>().GetPropertyBlock(propertyBlock);
            return propertyBlock.GetColor("_Color"); ;
        }
        private int  GetIndexFrom(Vector2 position)
        {
            if(position.x < 0 || position.y < 0) return -1;
            if(position.x >= _gridDimensions.x || position.y >= _gridDimensions.y) return -1;
            return (int) position.y * (int) _gridDimensions.y + (int) position.x;
        }

        #endregion

        #region Privates & Protected

        [SerializeField]
        private Vector2Int _gridDimensions;
        private GameObject[,] _cellPopulationList;
        private bool[,] _cellPopulationNextState;
        [SerializeField] private float _prefabScale = 0.5f;
        [SerializeField] GameObject _cell;
        [SerializeField] Color  cellAliveColor = Color.yellow;
        [SerializeField] Color  cellDeadColor = Color.blue;
        [SerializeField] float _delayGeneration = 0;
        [SerializeField] TMP_Text _aliveCellCountUI;
        [SerializeField] TMP_Text _genereationCountUI;
        float _currentTimer = 0;
        [Range(0f, 1f), SerializeField]
        private float _probabilityOfLifeAtInit;
        private int _cellPopulation = 0;
        private int _cellGeneration = 1;
        private bool _toggleAutoMode = false;
        MaterialPropertyBlock _propertyBlockDead ;
        MaterialPropertyBlock _propertyBlockAlive ;

        #endregion
    }

}
