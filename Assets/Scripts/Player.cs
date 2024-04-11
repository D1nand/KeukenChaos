using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;


    public event EventHandler<OnSelecterCounterChangedEventArgs> OnSelecterCounterChanged;

    public class OnSelecterCounterChangedEventArgs : EventArgs
    {

    }
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;


    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }
    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if(moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }


        float interactDistance = 2f;
        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
           if(raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if(clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {

            Vector2 inputVector = gameInput.GetMovementVectorNormalized();

            Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

            float moveDistance = moveSpeed * Time.deltaTime;
            float playerRadius = .6f;
            float playerHeight = 2f;
            bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

            if (!canMove)
            {
                Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

                if (canMove)
                {
                    moveDir = moveDirX;
                }
                else
                {
                    Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                    canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                    if (canMove)
                    {
                        moveDir = moveDirZ;
                    }
                    else
                    {

                    }
                }

            }
            if (canMove)
            {
                transform.position += moveDir * moveDistance;
            }

            isWalking = moveDir != Vector3.zero;
            float rotatespeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotatespeed);
        }
    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelecterCounterChanged?.Invoke(this, new OnSelecterCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }
    }