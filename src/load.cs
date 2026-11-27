using System; using System.IO; using System.Linq; using System.Reflection; using System.Collections.Generic;
using FrostySdk; using FrostySdk.Managers; using FrostySdk.Interfaces;
public class ConLog : ILogger { public void Log(string t, params object[] v){ var s=string.Format(t,v); if(!s.StartsWith("progress")) Console.WriteLine(s);} public void LogWarning(string t, params object[] v){Console.WriteLine("WARN "+string.Format(t,v));} public void LogError(string t, params object[] v){Console.WriteLine("ERR "+string.Format(t,v));} }
public static class Loader {
 public const string D=@"D:\Tools\FrostyEditor\FrostyEditor"; public const string G=@"D:\SteamLibrary\steamapps\common\Need for Speed Heat";
 public static void Hook(){ AppDomain.CurrentDomain.AssemblyResolve+=(s,e)=>{var n=new AssemblyName(e.Name).Name; if(n=="EbxClasses") return Assembly.LoadFrom(Path.Combine(D,"Profiles","NFSHEATSDK.dll")); foreach(var p in new[]{Path.Combine(D,n+".dll"),Path.Combine(D,"ThirdParty",n+".dll"),Path.Combine(D,"Profiles",n+".dll")}) if(File.Exists(p)) return Assembly.LoadFrom(p); return null;}; }
 public static AssetManager Load(){
  Directory.SetCurrentDirectory(D);
  ProfilesLibrary.Initialize(new List<Profile>());
  if(!ProfilesLibrary.Initialize("NeedForSpeedHeat")) throw new Exception("profile");
  Console.Error.WriteLine("profile ok");
  var fs=new FileSystem(G); foreach(var src in ProfilesLibrary.Sources) fs.AddSource(src.Path, src.SubDirs);
  Console.Error.WriteLine("sources ok");
  fs.Initialize(File.ReadAllBytes(Path.Combine(D,"NFSHEAT.key")));
  Console.Error.WriteLine("fs ok"); TypeLibrary.Initialize(true); Console.Error.WriteLine("types ok");
  var rm=new ResourceManager(fs); rm.SetLogger(new ConLog()); rm.Initialize();
  Console.Error.WriteLine("rm ok");
  var am=new AssetManager(fs,rm); am.SetLogger(new ConLog()); am.Initialize(true,new AssetManagerImportResult());
  return am; }
}
