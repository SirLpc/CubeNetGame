class HumanPlayer : HexPlayer
{
    public override void Play(CellGrid cellGrid)
    {
        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
    }
}