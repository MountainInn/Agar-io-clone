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
    private float radius;
    private CellGroup cellGroup;

    [SerializeField]
    private CircleCollider2D foodCollider, cellCollider;

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

    private void Awake()
    {
        Grow();
    }

    private void SetDirection(Vector3 mousePosition)
    {
        var cameraPosition = Camera.main.transform.position;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition -= cameraPosition;
        mousePosition.Scale(new Vector3(int.MaxValue, int.MaxValue, 0));
        mousePosition.Normalize();
        this.movementDirection = mousePosition;
    }

    private void Eject()
    {
        if (mass < ejectMassThreshold)
            return;

        var spawnPosition = GetMembranePoint();

        var newFood = GameObject.Instantiate(GetComponent<Food>(), spawnPosition, Quaternion.identity);

        mass -= ejectMass;

        newFood.mass = ejectMass;
    }

    private void Split()
    {
        var spawnPosition = GetMembranePoint();
        var newCell = GameObject.Instantiate(GetComponent<Cell>(), spawnPosition, Quaternion.identity);

        var splitMass = mass / 2;

        this.mass = newCell.mass = splitMass;
    }

    private Vector3 GetMembranePoint()
    {
        return transform.position + movementDirection * radius;
    }

    private void Grow()
    {
        radius = mass / 2;
        foodCollider.radius = radius * .95f;
        cellCollider.radius = radius * .85f;
        speed = 2;
    }

    private void FixedUpdate()
    {
        var step = movementDirection * speed * Time.fixedDeltaTime;
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

