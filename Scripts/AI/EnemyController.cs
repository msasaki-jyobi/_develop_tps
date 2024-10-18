using develop_common;
using develop_tps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private UnitActionLoader _actionLoader;
    [SerializeField] private float _span = 3f;
    [SerializeField] private int _aiID = 0;

    public List<EnemySkillInfo> SkillActions = new List<EnemySkillInfo>();

    private float _timer;
    private bool _isAction;

    private void Update()
    {
        _timer += Time.deltaTime;

        if(_timer > _span)
        {
            int ran = Random.Range(0, 4);
            _aiID = ran;
            _timer = 0;
            _isAction = false;
        }

        switch (_aiID)
        {
            case 0:
                Wait();
                break;
            case 1:
                Dash();
                break;
            case 2:
                SkillAction();
                break;
            case 3:
                Look();
                break;
        }
    }

    private void Wait()
    {

    }

    private void Dash()
    {
        transform.LookAt(_target.transform, transform.up);
        transform.Translate(0, 0, 3 * Time.deltaTime);
    }

    private void SkillAction()
    {
        if(!_isAction)
        {
            var skill = SearchSkill(3f);

            if (skill != null)
            {
                _isAction = true;
                _actionLoader.LoadAction(skill.SkillAction);
            }
        }
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
        transform.LookAt(_target.transform, transform.up);
    }

}
