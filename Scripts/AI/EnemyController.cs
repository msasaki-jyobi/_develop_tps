using develop_common;
using develop_tps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject Target;
    public Animator Animator;
    [SerializeField] private UnitActionLoader _actionLoader;
    [SerializeField] private float _span = 3f;
    [SerializeField] private int _aiID = 0;

    public List<EnemySkillInfo> SkillActions = new List<EnemySkillInfo>();

    private float _timer;
    private bool _isAction;

    private void Start()
    {
        if (Target == null)
            Target = GameObject.Find("Player");
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        //if(_timer > _span)
        //{
        //    int ran = Random.Range(0,3);
        //    _aiID = ran;
        //    _timer = 0;
        //    _isAction = false;
        //}

        if(_timer > 0 && _timer <= 2)
        {
            _isAction = false;
            Animator.SetFloat("Blend", 1f);
            Look();
            transform.Translate(0, 0, 3 * Time.deltaTime);
        }
        else if (_timer > 2 && _timer <= 4)
        {
            Animator.SetFloat("Blend", 0f);
        }
        else if (_timer > 4 && _timer <= 6)
        {
            if (!_isAction)
            {
                var skill = SearchSkill(100f);
                if (skill != null)
                {
                    _isAction = true;
                    _timer = 0;

                    Animator.SetFloat("Blend", 0f);
                    _actionLoader.LoadAction(skill.SkillAction, EInputReader.R1);
                }
            }
        }
        else
        {
            _timer = 0;
        }
    }

    private void UpdateTest()
    {
        _timer += Time.deltaTime;

        if (_timer > _span)
        {
            int ran = Random.Range(0, 3);
            _aiID = ran;
            _timer = 0;
            _isAction = false;
        }

        switch (_aiID)
        {
            case 0:
                Look();
                break;
            case 1:
                DistanceSkillPlay();
                break;
            case 2:
                RandomSkillPlay();
                break;
        }
    }

    private void Dash()
    {

        var targetDistance = Vector3.Distance(transform.position, Target.transform.position);
        if (targetDistance <= 0.1f)
        {
            Animator.SetFloat("Blend", 0f);
            return;
        }
        Look();
        transform.Translate(0, 0, 3 * Time.deltaTime);
        Animator.SetFloat("Blend", 1f);
    }

    private void RandomSkillPlay()
    {
        if (!_isAction)
        {
            var skill = SearchSkill(100f);
            if (skill != null)
            {
                _isAction = true;
                _actionLoader.LoadAction(skill.SkillAction, EInputReader.R1);
                Animator.SetFloat("Blend", 0f);
                return;
            }
        }
            Dash();
    }

    private void DistanceSkillPlay()
    {
        if (!_isAction)
        {
            var targetDistance = Vector3.Distance(transform.position, Target.transform.position);
            var skill = SearchSkill(targetDistance);

            if (skill != null)
            {
                _isAction = true;
                _actionLoader.LoadAction(skill.SkillAction, EInputReader.R1);
                Animator.SetFloat("Blend", 0f);
                return;
            }
        }
        Dash();
    }


    /// <summary>
    /// 発動スキルを一つ選定する
    /// </summary>
    /// <param name="targetDistance">ターゲットとの距離</param>
    private EnemySkillInfo SearchSkill(float targetDistance)
    {
        // LINQを使って発動可能なスキルを選定
        var availableSkills = SkillActions
            .Where(skill => skill.Distance >= targetDistance) // ターゲット距離以内のスキルを選定
            .ToList(); // リストに変換

        // 利用可能なスキルが存在する場合、その中からランダムで1つ選定
        if (availableSkills.Any())
        {
            var selectedSkill = availableSkills[UnityEngine.Random.Range(0, availableSkills.Count)];
            Debug.Log("選定されたスキル: " + selectedSkill.SkillAction.name);

            // スキルを発動する処理をここに記述
            //ActivateSkill(selectedSkill.SkillAction);
            return selectedSkill;
        }
        else
        {
            Debug.Log("発動可能なスキルがありません。");
            return null;
        }
    }


    private void Look()
    {
        Vector3 direction = Target.transform.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            Animator.SetFloat("Blend", 0f);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // スムーズに回転するためにSlerpを使用
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

}
