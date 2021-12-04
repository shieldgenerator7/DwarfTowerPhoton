



public enum ButtonState
{
    NONE,
    DOWN,
    HELD,
    UP
}

public static class ButtonStateUtils
{
    //2021-12-03: copied from https://stackoverflow.com/a/6651404/2336212
    public static bool Bool(this ButtonState value)
    {
        switch (value)
        {
            case ButtonState.DOWN:
            case ButtonState.UP:
            case ButtonState.HELD:
                return true;
            case ButtonState.NONE:
                return false;
        }
        throw new System.ArgumentException(
            $"ButtonState not recognized! buttonState: {value}"
            );
    }
}
