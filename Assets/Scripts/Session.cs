using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session : MonoBehaviour
{
    public static Session _Session;
    public enum StateSission
    {
        //Состояния запуска
        None,
        Settings,           //Настройка сессии
        SleepStart,         //Временная задержка перед запуском
        RandomDir,          //Выбор стартового рандомного значения

        //Состояния цикла
        ChecksAndFinMovePoint,// Проверки, определяем следующею точку перемещения

        //Состояние конца сессии
        EndSession          //Конец сессии
    }
    public StateSission _StateSission = StateSission.None;

    public delegate void EventSession();
    public event EventSession _StartSession;
    public event EventSession _EndSession;
    public bool _IsPlaySession = false;

    public int _Experience = 0;
    public int _Record = 0;
    public int _AddExperience = 10;

    public float _StartSpeed = 1;
    public float _StartR = 0.025f;
    public float _StartLength = 0.25f;
    public float _StartSpeedRacket = 1;

    private float _Speed;
    private float _R;
    private float _Length;
    private float _SpeedRacket;

    public float _AddSpeed = 0.1f;
    public float _TimeAddSpeed = 1;
    private float _OldTime = 0;

    private static GameObject _BallObj;
    private static GameObject _RacketObj;

    private Vector2[] _Point = new Vector2[4];

    public Color32 _ColorBall = new Color32(255, 255, 255, 255);
    private GameObject _Ball;
    private GameObject _Racket_1;
    private GameObject _Racket_2;

    private Vector2 _Dir;

    public void StartSession()
    {
        _IsPlaySession = true;
        _StateSission = StateSission.Settings;
    }
    public void StopSession()
    {
        EndSession();
    }

    void Awake()
    {
        _Session = this;
    }
    void Update()
    {
        switch (_StateSission)
        {
            case StateSission.Settings:
                Settings();
                break;
            case StateSission.SleepStart:
                SleepStart();
                break;
            case StateSission.RandomDir:
                RandomDir();
                break;
            case StateSission.ChecksAndFinMovePoint:
                ChecksAndFinMovePoint();
                break;
            case StateSission.EndSession:
                EndSession();
                break;
            default:
                break;
        }
    }

    private void Settings()
    {
        _Speed = _StartSpeed;
        _R = _StartR;
        _Length = _StartLength;
        _SpeedRacket = _StartSpeedRacket;
        _Experience = 0;

    _Record = PlayerPrefs.GetInt("Record");

        if (_BallObj == null)
            _BallObj = (GameObject)Resources.Load("Ball");
        if (_RacketObj == null)
            _RacketObj = (GameObject)Resources.Load("Racket");

        Camera.main.orthographicSize = 1;

        float n = 0;
        if (Screen.height > Screen.width)
            n = 1.0f / (Screen.height / (float)Screen.width);
        else
            n = 1.0f * (Screen.width / (float)Screen.height);

        _Point[0] = new Vector2(n * -1, -1);
        _Point[1] = new Vector2(n, -1);
        _Point[2] = new Vector2(n, 1);
        _Point[3] = new Vector2(n * -1, 1);

        if (_Ball == null)
        {
            _Ball = Instantiate(_BallObj);
            _Ball.transform.localScale = new Vector3(_R * 2, _R * 2, _R * 2);
            _Ball.name = "Ball";
            _Ball.transform.parent = transform;
            _Ball.transform.localPosition = Vector2.zero;
        }
        else
        {
            _Ball.transform.localPosition = Vector2.zero;
            _Ball.SetActive(true);
        }
        _Ball.GetComponent<SpriteRenderer>().color = _ColorBall;

        if (_Racket_1 == null)
        {
            _Racket_1 = Instantiate(_RacketObj);
            _Racket_1.transform.localScale = new Vector3(_Length, 0.05f, 1);
            _Racket_1.name = "Racket_1";
            _Racket_1.transform.parent = transform;
            _Racket_1.transform.localPosition = new Vector2(0, -1 + 0.05f);
        }
        else
        {
            _Racket_1.transform.localPosition = new Vector2(0, -1 + 0.05f);
            _Racket_1.SetActive(true);
        }

        if (_Racket_2 == null)
        {
            _Racket_2 = Instantiate(_RacketObj);
            _Racket_2.transform.localScale = new Vector3(_Length, 0.05f, 1);
            _Racket_2.name = "Racket_2";
            _Racket_2.transform.parent = transform;
            _Racket_2.transform.localPosition = new Vector2(0, 1 - 0.05f);
        }
        else
        {
            _Racket_2.transform.localPosition = new Vector2(0, 1 - 0.05f);
            _Racket_2.SetActive(true);
        }

        _StateSission = StateSission.SleepStart;

        _IsPlaySession = true;
        if (_StartSession != null)
            _StartSession();
    }
    private void SleepStart()
    {
        _StateSission = StateSission.RandomDir;
    }
    private void RandomDir()
    {
        _Dir = Random.insideUnitCircle.normalized;
        _StateSission = StateSission.ChecksAndFinMovePoint;
    }
    private void ChecksAndFinMovePoint()
    {
        if (Time.time - _OldTime > _TimeAddSpeed)
        {
            _OldTime = Time.time;
            _Speed += _AddSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _Racket_1.transform.position += (Vector3)(Vector2.right * (Time.deltaTime * _SpeedRacket));
            _Racket_2.transform.position += (Vector3)(Vector2.right * (Time.deltaTime * _SpeedRacket));

            if (_Racket_1.transform.position.x + (_Length / 2) > _Point[1].x || _Racket_2.transform.position.x + (_Length / 2) > _Point[1].x)
            {
                _Racket_1.transform.position = new Vector2(_Point[1].x - (_Length / 2), _Racket_1.transform.position.y);
                _Racket_2.transform.position = new Vector2(_Point[1].x - (_Length / 2), _Racket_2.transform.position.y);
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            _Racket_1.transform.position += (Vector3)(Vector2.left * (Time.deltaTime * _SpeedRacket));
            _Racket_2.transform.position += (Vector3)(Vector2.left * (Time.deltaTime * _SpeedRacket));

            if (_Racket_1.transform.position.x - (_Length / 2) < _Point[0].x || _Racket_2.transform.position.x - (_Length / 2) < _Point[0].x)
            {
                _Racket_1.transform.position = new Vector2(_Point[0].x + (_Length / 2), _Racket_1.transform.position.y);
                _Racket_2.transform.position = new Vector2(_Point[0].x + (_Length / 2), _Racket_2.transform.position.y);
            }
        }

        Vector2 nextVect = _Dir * (Time.deltaTime * _Speed);

        Point point = CheckRacket((Vector2)_Ball.transform.position + nextVect);
        if (point != null)
        {
            _Ball.transform.position = new Vector3(point.x, point.y, 0);
            goto m1;
        }

        if (!CheckGameOver((Vector2)_Ball.transform.position + nextVect))
        {
            _StateSission = StateSission.EndSession;
            goto m1;
        }

        point = CheckLineVertical((Vector2)_Ball.transform.position + nextVect);
        if (point != null)
        {
            _Ball.transform.position = new Vector3(point.x, point.y, 0);
            goto m1;
        }

        _Ball.transform.position += (Vector3)nextVect;
    m1:;
    }
    private void EndSession()
    {
        if(_Record < _Experience)
            PlayerPrefs.SetInt("Record", _Experience);

        _Ball.SetActive(false);
        _Racket_1.SetActive(false);
        _Racket_2.SetActive(false);

        _StateSission = StateSission.None;
        _IsPlaySession = false;

        if(_EndSession != null)
            _EndSession();
    }

    private Point CheckRacket(Vector2 nextPoint)
    {
        Point point = null;
        Vector2 oldDir = _Dir;
        if (_Dir.y < 0)
        {
            point = Point.FindPoint(_Ball.transform.position,
                                    nextPoint + (_Dir * _R),
                                    (Vector2)_Racket_1.transform.position + Vector2.right * (_Length / 2),
                                    (Vector2)_Racket_1.transform.position - Vector2.right * (_Length / 2));
            /*if(point == null)
                point = Point.FindPoint((Vector2)_Ball.transform.position + Vector2.right * _R,
                                    (nextPoint + Vector2.right * _R) + (_Dir * _R),
                                    (Vector2)_Racket_1.transform.position + Vector2.right * (_Length / 2),
                                    (Vector2)_Racket_1.transform.position - Vector2.right * (_Length / 2));
            if (point == null)
                point = Point.FindPoint((Vector2)_Ball.transform.position + Vector2.left * _R,
                                    (nextPoint + Vector2.left * _R) + (_Dir * _R),
                                    (Vector2)_Racket_1.transform.position + Vector2.right * (_Length / 2),
                                    (Vector2)_Racket_1.transform.position - Vector2.right * (_Length / 2));*/

            if (point != null)
            {
                _Dir = Vector2.Reflect(_Dir, Vector2.up);
            }
        }
        else if (_Dir.y > 0)
        {
            point = Point.FindPoint(_Ball.transform.position,
                                    nextPoint + (_Dir * _R),
                                    (Vector2)_Racket_2.transform.position + Vector2.right * (_Length / 2),
                                    (Vector2)_Racket_2.transform.position - Vector2.right * (_Length / 2));
            /*if (point == null)
                point = Point.FindPoint((Vector2)_Ball.transform.position + Vector2.right * _R,
                                    (nextPoint + Vector2.right * _R) + (_Dir * _R),
                                    (Vector2)_Racket_2.transform.position + Vector2.right * (_Length / 2),
                                    (Vector2)_Racket_2.transform.position - Vector2.right * (_Length / 2));
            if (point == null)
                point = Point.FindPoint((Vector2)_Ball.transform.position + Vector2.left * _R,
                                    (nextPoint + Vector2.left * _R) + (_Dir * _R),
                                    (Vector2)_Racket_2.transform.position + Vector2.right * (_Length / 2),
                                    (Vector2)_Racket_2.transform.position - Vector2.right * (_Length / 2));*/
            if (point != null)
            {
                _Dir = Vector2.Reflect(_Dir, Vector2.down);
            }
        }

        if (point != null)
        {
            Vector2 pointV = new Vector2(point.x, point.y) - oldDir * _R;
            point = new Point(pointV.x, pointV.y);
            _Experience += _AddExperience;
        }

        return point;
    }
    private bool CheckGameOver(Vector2 nextPoint)
    {
        if (nextPoint.y - _R <= _Point[0].y)
            return false;
        else if ((nextPoint.y + _R >= _Point[2].y))
            return false;
        else
            return true;
    }
    private Point CheckLineVertical(Vector2 nextPoint)
    {
        Point point = null;

        if (nextPoint.x - _R <= _Point[0].x)
        {
            point = new Point(_Point[0].x + _R, nextPoint.y);
            _Dir = Vector2.Reflect(_Dir, Vector2.right);
        }
        else if ((nextPoint.x + _R >= _Point[1].x))
        {
            point = new Point(_Point[1].x - _R, nextPoint.y);
            _Dir = Vector2.Reflect(_Dir, Vector2.left);
        }

        return point;
    }
}
public class Point
{
    public float x;
    public float y;

    public Point(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static float VectorMult(float ax, float ay, float bx, float by)
    {
        return ax * by - bx * ay;
    }
    public static bool Crossing(Point p1, Point p2, Point p3, Point p4)
    {
        float v1 = VectorMult(p4.x - p3.x, p4.y - p3.y, p1.x - p3.x, p1.y - p3.y);
        float v2 = VectorMult(p4.x - p3.x, p4.y - p3.y, p2.x - p3.x, p2.y - p3.y);
        float v3 = VectorMult(p2.x - p1.x, p2.y - p1.y, p3.x - p1.x, p3.y - p1.y);
        float v4 = VectorMult(p2.x - p1.x, p2.y - p1.y, p4.x - p1.x, p4.y - p1.y);

        if ((v1 * v2) < 0 && (v3 * v4) < 0)
            return true;
        return false;
    }
    public static float[] LineEquation(Point p1, Point p2)
    {
        float A = p2.y - p1.y;
        float B = p1.x - p2.x;
        float C = -p1.x * (p2.y - p1.y) + p1.y * (p2.x - p1.x);

        return new float[] { A, B, C };
    }
    public static Point CrossingPoint(float a1, float b1, float c1, float a2, float b2, float c2)
    {
        float d =  (a1 * b2 - b1 * a2);
        float dx = (-c1 * b2 + b1 * c2);
        float dy = (-a1 * c2 + c1 * a2);
        float X = (dx / d);
        float Y = (dy / d);
        return new Point(X, Y);
    }
    public static Point FindPoint(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        Point p1 = new Point(v1.x, v1.y);
        Point p2 = new Point(v2.x, v2.y);
        Point p3 = new Point(v3.x, v3.y);
        Point p4 = new Point(v4.x, v4.y);

        if (p2.x < p1.x)
        {
            Point e = p1;
            p1 = p2;
            p2 = e;
        }
        if (p4.x < p3.x)
        {
            Point e = p3;
            p3 = p4;
            p4 = e;
        }

        if (Crossing(p1, p2, p3, p4))
        {
            float a1, b1, c1, a2, b2, c2;
            float[] line_1 = LineEquation(p1, p2);
            a1 = line_1[0]; b1 = line_1[1]; c1 = line_1[2];
            float[] line_2 = LineEquation(p3, p4);
            a2 = line_2[0]; b2 = line_2[1]; c2 = line_2[2];
            return CrossingPoint(a1, b1, c1, a2, b2, c2);
        }
        else
        {
            return null;
        }
    }
}