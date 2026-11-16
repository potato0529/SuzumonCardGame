using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// PlayerCommandのシリアライズ処理のためのラッパー(今はjson式だが今後違う方式になったとしても(デ)シリアライズしてほしいという命令さえ変わらなければここを通ることになる。変わるのはこのクラスのメソッドの中身のみ
/// </summary>
public static class CommandSerializer
{
    /// <summary>
    /// Commandをシリアライズする
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static string Serialize(PlayerCommand command)
    {
        return JsonConvert.SerializeObject(command, JsonSettings.Settings);
    }

    /// <summary>
    /// Commandをデシリアライズする
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static PlayerCommand Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<PlayerCommand>(json,JsonSettings.Settings);
    }
}
