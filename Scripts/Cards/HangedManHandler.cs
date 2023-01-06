using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnboundLib;
using ModsPlus;
using Photon.Pun;
using UnboundLib.GameModes;
using UnityEngine.Events;
using PlayerTimeScale;

public class HangedManHandler : PlayerHook
{
    [SerializeField]
    private PlayerTimeScale.PlayerTimeScale slowEffect;

    [SerializeField]
    private GameObject applySlowEffect;

    [SerializeField]
    private float duration, timeScale;
    
    [SerializeField]
    private UnityEvent onStart, onEnd;

    private bool spent = false;

    private List<PlayerTimeScale.PlayerTimeScale> appliedDebuffs = new List<PlayerTimeScale.PlayerTimeScale>();

    protected override void Awake()
    {
        base.Awake();
        GetComponent<ParticleSystemColor>().color = player.GetTeamColors().winText;
    }

    protected override void Start()
    {
        base.Start();
        if (PhotonNetwork.OfflineMode)
        {
            StartCoroutine(OnBattleStart(null));
        }
    }

    private void Update()
    {
        if (spent) return;
        player.data.block.sinceBlock = 0f;
    }

    public override IEnumerator OnBattleStart(IGameModeHandler gameModeHandler)
    {
        if (spent) yield break;

        onStart?.Invoke();
        yield return MoveAndHold(player, Vector3.zero, duration);
        player.data.view.RPC("RPCA_Die", RpcTarget.All, Vector2.up);

        spent = true;
    }

    public override IEnumerator OnPointEnd(IGameModeHandler gameModeHandler)
    {
        spent = true;
        StopAllCoroutines();
        yield break;
    }

    private void ApplyEffect(Player target)
    {
        var debuff = Instantiate(slowEffect, target.transform);
        debuff.Scale = timeScale;

        var debuffColor = debuff.GetComponent<ParticleSystemColor>();
        debuffColor.color = target.GetTeamColors().winText;

        appliedDebuffs.Add(debuff);
    }
    
    private IEnumerator MoveAndHold(Player person, Vector3 targetPos, float holdTime)
    {
        PlayerVelocity playerVel = person.data.playerVel;
        AnimationCurve playerMoveCurve = PlayerManager.instance.playerMoveCurve;
        playerVel.SetFieldValue("simulated", false);
        playerVel.SetFieldValue("isKinematic", true);
        Vector3 distance = targetPos - playerVel.transform.position;
        Vector3 targetStartPos = playerVel.transform.position;
        PlayerCollision col = playerVel.GetComponent<PlayerCollision>();
        float t = playerMoveCurve.keys[playerMoveCurve.keys.Length - 1].time;
        float c = 0f;
        col.checkForGoThroughWall = false;
        while (c < t)
        {
            c += Mathf.Clamp(Time.unscaledDeltaTime, 0f, 0.02f);
            playerVel.transform.position = targetStartPos + distance * playerMoveCurve.Evaluate(c);
            yield return null;
        }

        var targets = PlayerManager.instance.players.Where(p => p.teamID != player.teamID).ToList();
        if (targets.Count == 0)
        {
            yield return new WaitForSeconds(holdTime);
        }
        for (int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];

            // spawn vfx
            var applyEffect = Instantiate(applySlowEffect, target.transform.position, applySlowEffect.transform.rotation);
            // make vfx follow target
            applyEffect.AddComponent<FollowTransform>().target = target.transform;
            // change clock hand and particle colors to match player team
            var color = target.GetTeamColors().winText;
            applyEffect.GetComponent<SpriteColor>().color = color;
            applyEffect.GetComponent<ParticleSystemColor>().color = color;
            
            // wait
            yield return new WaitForSeconds(holdTime / targets.Count);

            // apply time slow
            ApplyEffect(target);
        }

        onEnd?.Invoke();

        yield return new WaitForSeconds(1f);

        col.SetFieldValue("lastPos", (Vector2)targetPos);

        yield return null;

        col.checkForGoThroughWall = true;
        yield return null;

        int frames = 0;
        while (frames < 10)
        {
            playerVel.transform.position = targetPos;
            frames++;
            yield return null;
        }
        
        playerVel.SetFieldValue("simulated", true);
        playerVel.SetFieldValue("isKinematic", false);

        if (PhotonNetwork.OfflineMode)
        {
            spent = true;
        }

        yield break;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        foreach (var debuff in appliedDebuffs)
        {
            Destroy(debuff.gameObject);
        }
        appliedDebuffs.Clear();
    }
}
