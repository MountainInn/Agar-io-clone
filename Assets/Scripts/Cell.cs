using UnityEngine;
using Zenject;
using UniRx;
using Photon.Pun;
public class Cell : MonoBehaviourPunCallbacks
{
    private float ejectMass = 1;
    private float ejectMassThreshold = 3;
    private float splitMassThreshold;
    private float maxMass;
    private Vector3 movementDirection;
    private float _mass, speed;
    public float mass
    {
        get => _mass;
        set
        {
            _mass = value;

            Grow();
        }
    }
    private CellGroup cellGroup;

    private CompositeDisposable disposables;

    [Inject]
    public void Construct(CellController cellController, CellGroup cellGroup)
    {
        if (!photonView.IsMine)
            return;

        disposables = new CompositeDisposable();

        cellController.mousePositionStream
            .Subscribe(SetDirection)
            .AddTo(disposables);

        cellController.splitKeypressStream
            .Subscribe(_ => Split())
            .AddTo(disposables);

        cellController.ejectKeypressStream
            .Subscribe(_=> Eject())
            .AddTo(disposables);


        this.cellGroup = cellGroup;
    }
    new private void OnEnable()
    {
        base.OnEnable();
        if (!photonView.IsMine) return;

        cellGroup.IncrementCellCount();
    }
    new private void OnDisable()
    {
        base.OnDisable();
        if (!photonView.IsMine) return;

        cellGroup.DecrementCellCount();
        disposables.Clear();
    }

    private void SetDirection(Vector3 mousePosition)
    {
        var direction = (mousePosition - transform.position).normalize;
        this.movementDirection = direction;
    }

    private void Eject()
    {
        if (mass < ejectMassThreshold)
            return;

        var spawnPosition = GetMembranePoint();

        var newFood = GameObject.Instantiate(foodPrefab, Quaternion.identity, spawnPosition);

        mass -= ejectMass;

        newFood.mass = ejectMass;
    }

    private void Split()
    {
        var spawnPosition = GetMembranePoint();
        var newCell = GameObject.Instantiate(cellPrefab, Quaternion.identity, spawnPosition);

        var splitMass = mass / 2;

        this.mass = newCell.mass = splitMass;
    }

    private Vector3 GetMembranePoint()
    {
        return transform.position + movementDirection * circleCollider.radius;
    }

    private void Grow()
    {
        circleCollider.radius = mass / 2;
        speed = 200 - mass;
    }

    private void FixedUpdate()
    {
        var step = movementDirection * speed * Time.fixedDelta;
        transform.position = transform.position + step;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out Cell otherCell))
        {
            if (otherCell.photonView.IsMine)
                return;

            if (this.mass > otherCell.mass)
            {
                this.mass += otherCell.mass;
                GameObject.Destroy(otherCell.gameObject);
            }
        }
        else if (collider.gameObject.TryGetComponent(out Food food))
        {
            this.mass += food.mass;
            GameObject.Destroy(food.gameObject);
        }
    }
}

