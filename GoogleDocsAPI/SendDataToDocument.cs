using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;

class SendDataToDocument
{
  GoogleDocsCommunication googleDocsCommunication;

  private const int ONE_SECOND = 1000;

  public SendDataToDocument(GoogleDocsCommunication _googleDocsCommunication)
  {
    googleDocsCommunication = _googleDocsCommunication;
  }

  public async Task StartSendingText(DocsService docsService, TypeModes typeMode, string documentID, int delay, string text)
  {
    string[] words = GetWords(text);

    switch (typeMode)
    {
      case TypeModes.Normal:
        await SendingNormalModeAsync(docsService, documentID, delay, words);
        break;
      case TypeModes.Stealthy:
        await SendingStealthyModeAsync(docsService, documentID, delay, words);
        break;
      case TypeModes.SuperStealthy:
        await SendingSuperStealthyModeAsync(docsService, documentID, delay, words);
        break;
      default:
        await SendingNormalModeAsync(docsService, documentID, delay, words);
        break;
    }
  }

  async Task SendingNormalModeAsync(DocsService docsService, string documentID, int delay, string[] words)
  {
    foreach (string word in words)
    {
      await Task.Delay(delay);

      string docText = word;

      docText += " ";

      bool error = SendText(docsService, documentID, docText, GetEndIndex(docsService, documentID));

      if (error is true)
      {
        docsService = googleDocsCommunication.StartConnection(docsService.ApplicationName);

        await Task.Delay(ONE_SECOND);
      }
    }
  }

  async Task SendingStealthyModeAsync(DocsService docsService, string documentID, int delay, string[] words)
  {
    foreach (string word in words)
    {
      await Task.Delay(delay);

      string docWord = word;

      docWord += " ";

      char[] letters = docWord.ToCharArray();

      foreach (char letter in letters)
      {
        await Task.Delay(delay / letters.Length);

        string docText = letter.ToString();

        bool error = SendText(docsService, documentID, docText, GetEndIndex(docsService, documentID));

        if (error is true)
        {
          docsService = googleDocsCommunication.StartConnection(docsService.ApplicationName);

          await Task.Delay(ONE_SECOND);
        }
      }
    }
  }

  async Task SendingSuperStealthyModeAsync(DocsService docsService, string documentID, int delay, string[] words)
  {
    foreach (string word in words)
    {
      await Task.Delay(delay * 2);

      string docWord = word;

      docWord += " ";

      char[] letters = docWord.ToCharArray();

      foreach (char letter in letters)
      {
        Random random = new Random();

        await Task.Delay(random.Next(delay, (delay * 2) + 1));

        string docText = letter.ToString();

        bool error = SendText(docsService, documentID, docText, GetEndIndex(docsService, documentID));

        if (error is true)
        {
          docsService = googleDocsCommunication.StartConnection(docsService.ApplicationName);

          await Task.Delay(ONE_SECOND);
        }
      }
    }
  }

  bool SendText(DocsService _docsService, string documentID, string docText, int endIndex)
  {
    endIndex--;

    var requests = new BatchUpdateDocumentRequest
    {
      Requests = new[]
            {
                new Request
                {
                    InsertText = new InsertTextRequest
                    {
                        Location = new Location { Index = endIndex },
                        Text = docText
                    }
                }
        }
    };

    try
    {
      DocsService docsService = _docsService;

      docsService.Documents.BatchUpdate(requests, documentID).Execute();

      return false;
    }
    catch (Exception)
    {
      Console.WriteLine();
      Console.WriteLine($"An Error Occured At Index Position, {endIndex}");
      Console.WriteLine("Continuing The Program");

      return true;
    }
  }

  string[] GetWords(string text)
  {
    return text.Split(' ');
  }

  int GetEndIndex(DocsService docsService, string documentID)
  {
    Document doc = docsService.Documents.Get(documentID).Execute();

    return (int)doc.Body.Content.Where(x => x.EndIndex != null).Last().EndIndex;
  }
}