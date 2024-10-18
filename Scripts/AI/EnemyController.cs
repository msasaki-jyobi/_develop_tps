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
            int ran = Random.Range(0,3);
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
        var targetDistance = Vector3.Distance(transform.position, _target.transform.position);
        if (targetDistance <= 0.1f) return;

        transform.LookAt(_target.transform, transform.up);
        transform.Translate(0, 0, 3 * Time.deltaTime);
    }

    private void RandomSkillPlay()
    {
        if (!_isAction)
        {
            var skill = SearchSkill(100f);
            if (skill != null)
            {
                _isAction = true;
                _actionLoader.LoadAction(skill.SkillAction);
            }
        }
    }

    private void DistanceSkillPlay()
    {
        if (!_isAction)
        {
            var targetDistance = Vector3.Distance(transform.position, _target.transform.position);
            var skill = SearchSkill(targetDistance);

            if (skill != null)
            {
                _isAction = true;
                _actionLoader.LoadAction(skill.SkillAction);
            }
            else
            {
                Dash();
            }
        }
    }


    /// <summary>
    /// �����X�L������I�肷��
    /// </summary>
    /// <param name="targetDistance">�^�[�Q�b�g�Ƃ̋���</param>
    private EnemySkillInfo SearchSkill(float targetDistance)
    {
        // LINQ���g���Ĕ����\�ȃX�L����I��
        var availableSkills = SkillActions
            .Where(skill => skill.Distance >= targetDistance) // �^�[�Q�b�g�����ȓ��̃X�L����I��
            .ToList(); // ���X�g�ɕϊ�

        // ���p�\�ȃX�L�������݂���ꍇ�A���̒����烉���_����1�I��
        if (availableSkills.Any())
        {
            var selectedSkill = availableSkills[UnityEngine.Random.Range(0, availableSkills.Count)];
            Debug.Log("�I�肳�ꂽ�X�L��: " + selectedSkill.SkillAction.name);

            // �X�L���𔭓����鏈���������ɋL�q
            //ActivateSkill(selectedSkill.SkillAction);
            return selectedSkill;
        }
        else
        {
            Debug.Log("�����\�ȃX�L��������܂���B");
            return null;
        }
    }


    private void Look()
    {
        transform.LookAt(_target.transform, transform.up);
    }

}
