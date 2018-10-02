namespace VirtualSelf.CubeScripts
{
    public static class Scramble
    {
        public static void Easy(SideRotator rotator)
        {
            rotator.RotateFullTurns(Cube2X2.Side.Right, -1);
            rotator.RotateFullTurns(Cube2X2.Side.Bottom, -1);
            rotator.RotateFullTurns(Cube2X2.Side.Right, 1);
            rotator.RotateFullTurns(Cube2X2.Side.Bottom, 1);
        }

        public static void FixedFull(SideRotator rotator)
        {
            rotator.RotateFullTurns(Cube2X2.Side.Top, 2);
            rotator.RotateFullTurns(Cube2X2.Side.Front, 1);
            rotator.RotateFullTurns(Cube2X2.Side.Top, 2);
            rotator.RotateFullTurns(Cube2X2.Side.Right, 1);
            rotator.RotateFullTurns(Cube2X2.Side.Front, 2);
            rotator.RotateFullTurns(Cube2X2.Side.Right, -1);
            rotator.RotateFullTurns(Cube2X2.Side.Front, 1);
            rotator.RotateFullTurns(Cube2X2.Side.Right, -1);
            rotator.RotateFullTurns(Cube2X2.Side.Front, -1);
        }
    }
}
