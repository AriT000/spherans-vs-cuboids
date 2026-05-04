using UnityEngine;

/***************************************************************
*file: AimController.cs
*author: Nathan Rinon
*class: CS 4700 – Game Development
*assignment: program 1
*date last modified: 4/7/2026
*
*purpose: This script moves the user's crosshair by using vector normalization and adding an offset
*
****************************************************************/

using UnityEngine.InputSystem;



public class AimController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _crosshair_offset = 0f;
    private Vector2 _playerPos;
    private Vector2 _mousePos;
    private Camera _playerCam;
    public Vector2 _currentCrossHairPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        _playerCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        _mousePos = updateMousePos();
        _playerPos = _player.transform.position;
        _currentCrossHairPos = _mousePos;
        transform.position = _currentCrossHairPos;
    }

    private void OnDrawGizmos()
    {

    }

    //purpose: get mouse position relative to cursor on player screen
    private Vector2 updateMousePos()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 gameMousePos = _playerCam.ScreenToWorldPoint(mousePos);
        return gameMousePos;
    }

    //purpose: gets new crosshair position by normalizing the mouse position relative to player position
    /*private Vector2 getCrossHairPos(Vector2 playerPos, Vector2 mousePos)
    {
        Vector2 crossHairPos = playerPos + (mousePos - playerPos).normalized + new Vector2(_crosshair_offset, _crosshair_offset);

        return crossHairPos;
    } */
}

