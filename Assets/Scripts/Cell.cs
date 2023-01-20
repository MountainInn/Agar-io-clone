public class Cell : MonoBehaviourPunCallbacks
{
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

    [Inject]
    public void Construct(CellController cellController)
    {
        cellController.mousePositionStream
            .Subscribe(SetDirection);

        cellController.splitKeypressStream
            .Subscribe(_=> Split);
    }


    private void SetDirection(Vector3 mousePosition)
    {
        var direction = (mousePosition - transform.position).normalize;
        this.movementDirection = direction;
    }

    private void Split()
    {
        var spawnPosition = transform.position + movementDirection * circleCollider.radius;
        var newCell = GameObject.Instantiate(cellPrefab, Quaternion.identity, spawnPosition);

        var splitMass = mass / 2;

        this.mass = newCell.mass = splitMass;
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
        if (collider.gameObject is Cell otherCell)
        {
            if (otherCell.photonView.IsMine)
                return;

            if (this.mass > otherCell.mass)
            {
                this.mass += otherCell.mass;
                GameObject.Destroy(otherCell.gameObject);
            }
        }
        else if (collider.gameObject is Food food)
        {
            this.mass += food.mass;
            GameObject.Destroy(food.gameObject);
        }
    }
}
