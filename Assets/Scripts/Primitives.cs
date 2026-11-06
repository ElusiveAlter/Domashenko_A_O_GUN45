namespace Netologia.Tactical
{
    /// <summary>
    /// Цвет команды фишки (Player1/White и Player2/Black)
    /// </summary>
    public enum UnitColor
    {
        White,
        Black
    }

    /// <summary>
    /// Статус игры: Select — выбор фишки, Move — выбор клетки хода
    /// </summary>
    public enum GameStatus
    {
        Select,
        Move
    }
}