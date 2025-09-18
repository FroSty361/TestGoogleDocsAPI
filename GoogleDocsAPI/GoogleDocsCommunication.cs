using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using static System.Console;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

class GoogleDocsCommunication
{
  string[] scopes = { DocsService.Scope.Documents };

  const string CREDENTIALS_FILE = "credentials.json";

  public async Task AccessGoogleDoc(string applicationName, string documentID, TypeModes typeMode, int delay, string text)
  {
    DocsService docsService = StartConnection(applicationName);

    SendDataToDocument sendDataToDocument = new SendDataToDocument(this);

    await sendDataToDocument.StartSendingText(docsService, typeMode, documentID, delay, text);

    WriteLine("Text Written To Document");
  }

  public DocsService StartConnection(string applicationName)
  {
    UserCredential userCredential = GetUserCredentials();

    if (userCredential is null)
    {
      WriteLine("Could Not Access User Credentials");

      return null;
    }

    DocsService docsService = GetDocsService(userCredential, applicationName);

    if (docsService is null)
    {
      WriteLine("Could Not Get The Document");

      return null;
    }

    return docsService;
  }

  UserCredential GetUserCredentials()
  {
    using (var stream = new FileStream(CREDENTIALS_FILE, FileMode.Open, FileAccess.Read))
    {
      string credPath = "token.json";

      return GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(stream).Secrets, scopes, "user", CancellationToken.None, new FileDataStore(credPath, true)).Result;
    }
  }

  private DocsService GetDocsService(UserCredential userCredential, string applicationName)
  {
    return new DocsService(new BaseClientService.Initializer
    {
      HttpClientInitializer = userCredential,
      ApplicationName = applicationName,
    });
  }
}