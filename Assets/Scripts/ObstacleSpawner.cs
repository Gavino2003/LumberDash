using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] obstaclePrefabs;
    public GameObject mediumObstaclePrefab;
    public GameObject rollableObstaclePrefab;
    public GameObject logLeftPrefab;
    public GameObject logRightPrefab;

    [Header("Moedas")]
    public GameObject coinPrefab;
    public int coinsPerArc = 8;
    public float coinArcHeight = 2f;
    public float coinSpawnY = 0.5f;
    public float coinArcLength = 10f;

    [Header("Alturas de Spawn")]
    public float obstacleSpawnY = 0.5f;
    public float mediumObstacleSpawnY = 1f;
    public float rollableObstacleSpawnY = 0.5f;
    public float logSpawnY = 0.5f;

    [Header("Spawner")]
    public float spawnInterval = 2f;
    public float laneDistance = 2.5f;

    private float timer = 0f;
    private float spawnZ;
    private float[] lanePositions;

    const int NONE = 0;
    const int SMALL = 1;
    const int MEDIUM = 2;
    const int ROLLABLE = 3;
    const int LOG_LEFT = 4;
    const int LOG_RIGHT = 5;

    int[][] easyPatterns = new int[][]
    {
        new int[] { MEDIUM,   NONE,    NONE    },
        new int[] { NONE,     MEDIUM,  NONE    },
        new int[] { NONE,     NONE,    MEDIUM  },
        new int[] { MEDIUM,   NONE,    SMALL   },
        new int[] { SMALL,    NONE,    MEDIUM  },
        new int[] { MEDIUM,   SMALL,   NONE    },
        new int[] { NONE,     SMALL,   MEDIUM  },
        new int[] { NONE,     MEDIUM,  SMALL   },
        new int[] { SMALL,    MEDIUM,  NONE    },
    };

    int[][] mediumPatterns = new int[][]
    {
        new int[] { MEDIUM,   NONE,    MEDIUM   },
        new int[] { MEDIUM,   SMALL,   NONE     },
        new int[] { NONE,     SMALL,   MEDIUM   },
        new int[] { MEDIUM,   NONE,    SMALL    },
        new int[] { SMALL,    NONE,    MEDIUM   },
        new int[] { MEDIUM,   LOG_RIGHT, NONE   },
        new int[] { NONE,     LOG_LEFT,  MEDIUM },
        new int[] { NONE,     MEDIUM,  LOG_RIGHT},
        new int[] { LOG_LEFT, MEDIUM,  NONE     },
        new int[] { MEDIUM,   ROLLABLE, NONE    },
        new int[] { NONE,     ROLLABLE, MEDIUM  },
        new int[] { MEDIUM,   NONE,    ROLLABLE },
        new int[] { ROLLABLE, NONE,    MEDIUM   },
        new int[] { SMALL,    MEDIUM,  SMALL    },
    };

    int[][] hardPatterns = new int[][]
    {
        new int[] { MEDIUM,   SMALL,    MEDIUM   },
        new int[] { MEDIUM,   ROLLABLE, MEDIUM   },
        new int[] { MEDIUM,   LOG_RIGHT, SMALL   },
        new int[] { SMALL,    LOG_LEFT,  MEDIUM  },
        new int[] { LOG_LEFT, MEDIUM,   ROLLABLE },
        new int[] { ROLLABLE, MEDIUM,   LOG_RIGHT},
        new int[] { MEDIUM,   SMALL,    SMALL    },
        new int[] { SMALL,    SMALL,    MEDIUM   },
        new int[] { MEDIUM,   NONE,     MEDIUM   },
        new int[] { SMALL,    MEDIUM,   SMALL    },
        new int[] { MEDIUM,   ROLLABLE, SMALL    },
        new int[] { SMALL,    ROLLABLE, MEDIUM   },
    };

    void Start()
    {
        spawnZ = transform.position.z;
        lanePositions = new float[] { -laneDistance, 0f, laneDistance };
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsWaiting) return;
        timer += Time.deltaTime;

        float currentInterval = spawnInterval * (GameManager.Instance.initialSpeed / GameManager.Instance.CurrentSpeed);

        if (timer >= currentInterval)
        {
            timer = 0f;
            SpawnPattern();
        }
    }

    void SpawnPattern()
    {
        int[] pattern = ChoosePattern();

        for (int lane = 0; lane < 3; lane++)
        {
            switch (pattern[lane])
            {
                case SMALL:
                    SpawnAt(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], obstacleSpawnY, lane, Quaternion.identity);
                    break;
                case MEDIUM:
                    SpawnAt(mediumObstaclePrefab, mediumObstacleSpawnY, lane, Quaternion.identity);
                    break;
                case ROLLABLE:
                    SpawnAt(rollableObstaclePrefab, rollableObstacleSpawnY, lane, Quaternion.identity);
                    break;
                case LOG_LEFT:
                    SpawnAt(logLeftPrefab, logSpawnY, lane, logLeftPrefab.transform.rotation);
                    break;
                case LOG_RIGHT:
                    SpawnAt(logRightPrefab, logSpawnY, lane, logRightPrefab.transform.rotation);
                    break;
                case NONE:
                    SpawnCoins(lane);
                    break;
            }
        }
    }

    void SpawnCoins(int lane)
    {
        if (coinPrefab == null) return;
        if (Random.Range(0, 100) < 30) return;

        int pattern = Random.Range(0, 3);

        for (int i = 0; i < coinsPerArc; i++)
        {
            float t = (float)i / (coinsPerArc - 1);
            float x = lanePositions[lane];
            float z = spawnZ + (t * coinArcLength);
            float y = coinSpawnY;

            switch (pattern)
            {
                case 0: // arco
                    y = coinSpawnY + coinArcHeight * Mathf.Sin(t * Mathf.PI);
                    break;
                case 1: // linha reta
                    y = coinSpawnY;
                    break;
                case 2: // zigzag em altura
                    y = coinSpawnY + coinArcHeight * 0.5f * Mathf.Abs(Mathf.Sin(t * Mathf.PI * 3f));
                    break;
            }

            y = Mathf.Max(y, coinSpawnY);

            Instantiate(coinPrefab, new Vector3(x, y, z), Quaternion.identity);
        }
    }

    void SpawnAt(GameObject prefab, float spawnY, int lane, Quaternion rotation)
    {
        Vector3 spawnPos = new Vector3(lanePositions[lane], spawnY, spawnZ);
        Instantiate(prefab, spawnPos, rotation);
    }

    int[] ChoosePattern()
    {
        float speedRatio = (GameManager.Instance.CurrentSpeed - GameManager.Instance.initialSpeed) /
                           (GameManager.Instance.maxSpeed - GameManager.Instance.initialSpeed);

        int roll = Random.Range(0, 100);

        int hardThreshold   = Mathf.RoundToInt(Mathf.Lerp(5f,  60f, speedRatio));
        int mediumThreshold = Mathf.RoundToInt(Mathf.Lerp(50f, 90f, speedRatio));

        if (roll < hardThreshold)
            return hardPatterns[Random.Range(0, hardPatterns.Length)];

        if (roll < mediumThreshold)
            return mediumPatterns[Random.Range(0, mediumPatterns.Length)];

        return easyPatterns[Random.Range(0, easyPatterns.Length)];
    }
}