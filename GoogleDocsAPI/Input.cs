using System;
using static System.Console;

class Input
{
  const int DEFAULT_DELAY = 1000;

  public void InputDocument()
  {
    GoogleDocsCommunication googleDocsCommunication = new GoogleDocsCommunication();

    WriteLine("Input A Name For The Document");

    string documentName = ReadLine();

    WriteLine();

    WriteLine("Input A Document ID");

    string documentID = ReadLine();

    WriteLine();

    WriteLine("Input A Mode, {Normal}, {Stealthy}, {SuperStealthy}");

    string modeChoice = ReadLine();

    TypeModes typeMode;

    try
    {
      typeMode = Enum.Parse<TypeModes>(modeChoice);
    }
    catch
    {
      typeMode = TypeModes.Normal;
    }

    WriteLine();

    WriteLine("Input Time Betwwen Send, Milliseconds Integer");

    int delay;

    try
    {
      delay = Convert.ToInt32(ReadLine());
    }
    catch (Exception)
    {
      delay = DEFAULT_DELAY;
    }

    WriteLine();

    WriteLine("Input The Text");

    string text = ReadLine();

    WriteLine();

    googleDocsCommunication.AccessGoogleDoc(documentName, documentID, typeMode, delay, text);

    WriteLine();
  }
}