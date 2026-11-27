using System; using System.Collections; using System.Collections.Generic; using System.Linq; using System.Threading; using System.Text.RegularExpressions; using FrostySdk; using FrostySdk.Managers; using FrostySdk.Ebx; using FrostySdk.IO; using Frosty.Core; using Frosty.Core.Mod;
class Bd{ static void Main(string[] a){ Loader.Hook(); Go(a);} static object P(object o,string n){ return o.GetType().GetProperty(n).GetValue(o); }
 static void Go(string[] a){ float f=float.Parse(a[1],System.Globalization.CultureInfo.InvariantCulture); string outPath=a[2];
  var am=Loader.Load(); App.AssetManager=am; App.FileSystem=(FileSystem)typeof(AssetManager).GetField("fs",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(am); App.Logger=new ConLog();
  var names=System.IO.File.ReadAllLines(a[0]); var shRe=new Regex("^Shaders/World/Signs/SGN_",RegexOptions.IgnoreCase); var pRe=new Regex("intensity|scale|power",RegexOptions.IgnoreCase);
  var shaderName=new Dictionary<Guid,string>(); int meshes=0, pars=0;
  foreach(var n in names){ var e=am.GetEbxEntry(n); if(e==null){Console.WriteLine("missing "+n);continue;} var asset=am.GetEbx(e,false); bool ch=false;
   foreach(PointerRef pr in (IList)P(asset.RootObject,"Materials")){ if(pr.Type!=PointerRefType.Internal) continue; var shader=P(pr.Internal,"Shader"); var sref=(PointerRef)P(shader,"Shader"); if(sref.Type!=PointerRefType.External) continue;
    string sn; var g=sref.External.FileGuid; if(!shaderName.TryGetValue(g,out sn)){ var se=am.GetEbxEntry(g); sn=se==null?"":se.Name; shaderName[g]=sn; } if(!shRe.IsMatch(sn)) continue;
    foreach(var vp in (IList)P(shader,"VectorParameters")){ var pn=P(vp,"ParameterName").ToString(); if(pn.IndexOf("missive",StringComparison.OrdinalIgnoreCase)<0||!pRe.IsMatch(pn)) continue;
     var vprop=vp.GetType().GetProperty("Value"); var v=vprop.GetValue(vp); foreach(var c in new[]{"x","y","z","w"}){ var cp=v.GetType().GetProperty(c); cp.SetValue(v,(float)cp.GetValue(v)*f);} vprop.SetValue(vp,v); pars++; ch=true; } }
   if(ch){ am.ModifyEbx(e.Name,asset); meshes++; } }
  Console.WriteLine("modified meshes="+meshes+" params="+pars);
  Config.Load(a[3]); App.SelectedProfile="NeedForSpeedHeat";
  App.ResourceManager=(ResourceManager)typeof(AssetManager).GetField("rm",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(am);
  App.PluginManager=new PluginManager(new ConLog(), PluginManagerType.Editor);
  var proj=new FrostyProject(); var ms=proj.ModSettings; ms.Title="Glowboard Bloom Fix"; ms.Author="Custom"; ms.Category="Visuals"; ms.Version="1.0";
  ms.Description="Lowers the glow (EmissiveIntensity and related values) of world signs and billboards using Shaders/World/Signs/SGN_* shaders to "+(f*100)+"% of vanilla, which reduces their bloom at night. Does not touch lighting/VE files, so it works with any night preset. Place it at the bottom of the load order.";
  proj.WriteToMod(outPath, ms, false, CancellationToken.None); Console.WriteLine("wrote "+outPath+" "+new System.IO.FileInfo(outPath).Length); } }
