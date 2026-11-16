using UnityEngine;

public class PlayerCommand
{
    public byte player;

    public PlayerCommand(Player _player)
    {
        player = MatchManager.instance.GetPlayerId(_player);
    }
}

public class UseCardNormalCommand : PlayerCommand, IActionCommand
{
    public int cardId;
    public SerializableVector3 setPos;

    public UseCardNormalCommand(Player _player,int _setCardId,Vector3 _setPos) : base(_player)
    {
        cardId = _setCardId;
        setPos = new SerializableVector3(_setPos);         
    }
}

public class UseCardEffectCommand : PlayerCommand
{
    public int useCardId;
    public Vector3 setCardPos;
    public int searchingId;

    public UseCardEffectCommand(Player _player,int _useCardId, Vector3 _setCardPos, int _searchingId) : base(_player)
    {
        useCardId = _useCardId;
        setCardPos = _setCardPos;
        searchingId = _searchingId;
    }    
}

public class EffectButtonCommand : PlayerCommand,IActionCommand
{
    public int effectInvokerCardId;

    public EffectButtonCommand(Player _player, int _effectInvokerCardId) : base(_player)
    {
        effectInvokerCardId = _effectInvokerCardId;
    }
}

public class SelectCardCommand : PlayerCommand,IResponseCommand
{
    public int selectedCardId;
    public int selectingId;

    public SelectCardCommand(Player _player, int _selectedCardId, int _selectingId) : base(_player)
    {
        selectedCardId = _selectedCardId;
        selectingId = _selectingId;
    }
}

public class SelectSearchCardCommand : PlayerCommand,IResponseCommand
{
    public int selectedCardId;
    public int searchingId;

    public SelectSearchCardCommand(Player _player, int _selectedCardId, int _selectingId) : base(_player)
    {
        selectedCardId = _selectedCardId;
        searchingId = _selectingId;
    }
}

public class UsedSearchCardCommand : PlayerCommand,IResponseCommand
{
    public int usedSearchCardId;
    public SerializableVector3 setPos;
    public int searchingId;

    public UsedSearchCardCommand(Player _player, int _usedSearchCardId, Vector3 _setPos, int _searchingId) : base(_player)
    {
        usedSearchCardId = _usedSearchCardId;
        setPos = new SerializableVector3(_setPos);
        searchingId = _searchingId;
    }
}

public class ChoseEffectCommand : PlayerCommand,IResponseCommand
{
    public int choseEffectNum;

    public ChoseEffectCommand(Player _player, int _choseEffectNum) : base(_player)
    {
        choseEffectNum = _choseEffectNum;
    }
}

public class PhaseButtonCommand : PlayerCommand,IActionCommand
{

    public PhaseButtonCommand(Player _player) : base(_player)
    {
        
    }
}

public class StartAttackCommand : PlayerCommand,IActionCommand
{
    public int attackerCardId;
    public StartAttackCommand(Player _player, int _attackerCardId) : base(_player)
    {
        attackerCardId =_attackerCardId;
    }
}

public class DefenceCommand : PlayerCommand,IActionCommand
{
    public int defencerCardId;
    public DefenceCommand(Player _player, int _defencerCardId) : base(_player)
    {
        defencerCardId = _defencerCardId;
    }
}

public class AdventButtonCommand : PlayerCommand,IActionCommand
{
    public int adventCardId;
    public AdventButtonCommand(Player _player, int _adventCardId) : base(_player)
    {
        adventCardId = _adventCardId;
    }
}

public class YesNoCommand : PlayerCommand, IResponseCommand
{
    public bool isYes;

    public YesNoCommand(Player _player,bool _isYes) : base(_player)
    {
        isYes = _isYes;
    }
}

public class SpaceDrawCommand : PlayerCommand,IActionCommand
{
    public SpaceDrawCommand(Player _player) : base(_player)
    {
        
    }
}

public class SurrenderCommand : PlayerCommand,IActionCommand
{
    public SurrenderCommand(Player _player) : base(_player)
    {

    }
}

/// <summary>
/// キューに追加して順番に実行しなければならないコマンドのマーカーインターフェース。これに分類されるコマンドはなにも試合処理が行われていない(MatchManager.runningProcess == 0のとき)だけ実行できるターン終了宣言や手札から権利に依存したカードを使用するようなものが該当する。
/// </summary>
public interface IActionCommand
{

}

/// <summary>
/// 試合処理キューには追加せず、処理中に実行されうるカード対象選択やYesNo、ChooseEffectなどの情報をもつコマンドのマーカーインターフェース。
/// </summary>
public interface IResponseCommand
{

}

/// <summary>
/// その他に分類されるコマンドのマーカーインターフェース。
/// </summary>
public interface IOtherCommand
{

}