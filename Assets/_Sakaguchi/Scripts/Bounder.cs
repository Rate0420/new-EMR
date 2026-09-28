using System.Collections.Generic;
using UnityEngine;

public class Bounder : MonoBehaviour
{
    // Ball�^�O�����I�u�W�F�N�g���Փ˂����Ƃ��ɁA�Փ˂����I�u�W�F�N�g�𒵂˕Ԃ�
    // �N���[���̒��S���̃I�u�W�F�N�g�ɂ��̃X�N���v�g���A�^�b�`����
    // Vector3�ŃN���[���̊e���̈ʒu�Abool�Ō��Ƀ{�[���������Ă��邩�ǂ������Ǘ�����
    // ���˕Ԃ������́A�󂢂Ă��錊�̒��Ń����_���Ȍ��Ɍ������Ē��˕Ԃ��悤�ɂ���

    public GameObject[] HoleObjects; // �N���[���̌��̃I�u�W�F�N�g���i�[����z��
    Vector3[] holePositions; // �N���[���̌��̈ʒu���i�[����z��
    public bool[] holeOccupied; // �e�����{�[���Ő�L����Ă��邩�ǂ������i�[����z��
    public float bounceForce = 10f; // ���˕Ԃ��͂̑傫��
    public List<GameObject> Balls = new List<GameObject>();
    float timer;
    float interval = 4f; // 4�b����
    
    private void Start()
    {
        // holeOcupied,holeposition�z��̃T�C�Y��HoleObjects�̐��ɍ��킹�ď�����
        holeOccupied = new bool[HoleObjects.Length];
        holePositions = new Vector3[HoleObjects.Length];
        for (int i = 0; i < HoleObjects.Length; i++)
        {
            holePositions[i] = HoleObjects[i].transform.position;
            // holeOccupied�z��̏�����
            holeOccupied[i] = false;
        }
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            // 4�b���ƂɁA�󂢂Ă��錊�̒��Ń����_���Ȍ��Ɍ������Ē��˕Ԃ�
            int holeIndex = GetRandomHoleIndex();
            if (holeIndex != -1)
            {
                foreach (GameObject ball in Balls)
                {
                    if (ball != null)
                    {
                        Rigidbody rb = ball.GetComponent<Rigidbody>();
                        Vector3 direction = holePositions[holeIndex] - transform.position;
                        direction.y = 0;
                        direction.Normalize();
                        if (rb != null)
                        {
                            // ��U���x�A��]�����Z�b�g���Ă��璵�˕Ԃ�
                            rb.linearVelocity = Vector3.zero;
                            rb.angularVelocity = Vector3.zero;
                            rb.AddForce(direction * bounceForce, ForceMode.Impulse);
                        }
                    }
                }
            }
            timer = 0f;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.transform.parent.CompareTag("Ball"))
    //    {
    //        // �Փ˂����I�u�W�F�N�g��Ball�^�O�����ꍇ�A�󂢂Ă��錊�̒��Ń����_���Ȍ��Ɍ������Ĕ�΂��悤�ɂ���
    //        int holeIndex = GetRandomHoleIndex();
    //        if (holeIndex != -1)
    //        {
    //            Debug.Log("�Փ�");

    //            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
    //            Vector3 direction = holePositions[holeIndex] - transform.position;
    //            direction.y = 0;
    //            direction.Normalize();
    //            if (rb != null)
    //            {
    //                // ��U���x�A��]�����Z�b�g���Ă��璵�˕Ԃ�
    //                rb.linearVelocity = Vector3.zero;
    //                rb.angularVelocity = Vector3.zero;
    //                rb.AddForce(direction * bounceForce, ForceMode.Impulse);
    //            }
    //        }
    //    }
    //}

    int GetRandomHoleIndex()
    {
        // �󂢂Ă��錊�̒��Ń����_���Ȍ��̃C���f�b�N�X��Ԃ�
        // ���ׂĂ̌�����L����Ă���ꍇ��-1��Ԃ�
        int[] availableHoles = new int[holeOccupied.Length];
        int count = 0;
        for (int i = 0; i < holeOccupied.Length; i++)
        {
            if (!holeOccupied[i])
            {
                availableHoles[count] = i;
                count++;
            }
        }
        if (count > 0)
        {
            int randomIndex = Random.Range(0, count);
            Debug.Log("Available holes: " + count + ", Random hole index: " + availableHoles[randomIndex]);
            return availableHoles[randomIndex];
        }
        else
        {
            return -1;
        }
    }
}
