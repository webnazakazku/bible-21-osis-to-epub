﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using BibleDoEpubu.ObjektovyModel;
using ICSharpCode.SharpZipLib.Zip;

namespace BibleDoEpubu
{
  internal class EpubGenerator
  {
    #region Proměnné

    private const string KnihaPoznamky = "kniha-poznamky.html";

    private const string PodadresarHtml = "html";

    #endregion

    #region Vlastnosti

    private Dictionary<int, List<PouzitaPoznamka>> PouzitePoznamky
    {
      get;
      set;
    } = new Dictionary<int, List<PouzitaPoznamka>>();

    #endregion

    #region Metody

    public string VygenerovatKnihu(Kniha kniha, Bible bible, bool dlouheCislaVerse)
    {
      return VygenerovatCastTextu(kniha, kniha, bible, dlouheCislaVerse);
    }

    /// <summary>
    /// Vrací ID string pro vlastní text poznámky.
    /// </summary>
    /// <param name="kniha"></param>
    /// <param name="pozn"></param>
    /// <returns></returns>
    private static string ZiskatIdPoznamky(Kniha kniha, PouzitaPoznamka pozn)
    {
      return $"p-{kniha.Id}-{pozn.Id}";
    }

    /// <summary>
    /// Vrací ID string pro místo, kde je poznámka citovaná ([x]).
    /// </summary>
    /// <param name="kniha"></param>
    /// <param name="pozn"></param>
    /// <returns></returns>
    private static string ZiskatIdCitace(Kniha kniha, PouzitaPoznamka pozn)
    {
      return $"c-{kniha.Id}-{pozn.Id}";
    }

    private string VygenerovatCastTextu(CastTextu cast, Kniha kniha, Bible bible, bool dlouheCislaVerse)
    {
      if (cast is HlavniCastKnihy)
      {
        StringBuilder stavec = new StringBuilder();

        stavec.Append($"<h2>{((HlavniCastKnihy) cast).Nadpis}</h2>\n");

        foreach (CastTextu potomek in cast.Potomci)
        {
          stavec.Append(VygenerovatCastTextu(potomek, kniha, bible, dlouheCislaVerse));
        }

        return stavec.ToString();
      }
      else if (cast is CastKnihy)
      {
        StringBuilder stavec = new StringBuilder();

        stavec.Append($"<h4>{((CastKnihy) cast).Nadpis}</h4>\n");

        foreach (CastTextu potomek in cast.Potomci)
        {
          stavec.Append(VygenerovatCastTextu(potomek, kniha, bible, dlouheCislaVerse));
        }

        return stavec.ToString();
      }
      else if (cast is UvodKapitoly)
      {
        return $"<h3 id=\"{ZiskatIdKapitoly(((UvodKapitoly) cast).Id)}\">Kapitola {ZiskatKratkeCisloVerse(((UvodKapitoly) cast).Id)}</h3>\n";
      }
      else if (cast is Vers)
      {
        StringBuilder stavec = new StringBuilder();

        stavec.Append($"<sup>{(dlouheCislaVerse ? ZiskatDlouheCisloVerse(bible, (cast as Vers).Id) : ZiskatKratkeCisloVerse(((Vers) cast).Id))}</sup>");

        foreach (CastTextu potomek in cast.Potomci)
        {
          stavec.Append(VygenerovatCastTextu(potomek, kniha, bible, dlouheCislaVerse));
        }

        return stavec.ToString();
      }
      else if (cast is Poznamka)
      {
        StringBuilder stavec = new StringBuilder();

        foreach (CastTextu potomek in cast.Potomci)
        {
          stavec.Append(VygenerovatCastTextu(potomek, kniha, bible, dlouheCislaVerse));
        }

        int poradiKnihy = bible.Knihy.IndexOf(kniha);

        if (!PouzitePoznamky.ContainsKey(poradiKnihy))
        {
          PouzitePoznamky.Add(poradiKnihy, new List<PouzitaPoznamka>());
        }

        PouzitaPoznamka poznamka = new PouzitaPoznamka
        {
          Text = stavec.ToString(),
          Id = $"{PouzitePoznamky[poradiKnihy].Count + 1}"
        };

        PouzitePoznamky[poradiKnihy].Add(poznamka);

        return $"<sup class=\"poznamka\"><a id=\"{ZiskatIdCitace(kniha, poznamka)}\" href=\"{KnihaPoznamky}#{ZiskatIdPoznamky(kniha, poznamka)}\" epub:type=\"noteref\">[{PouzitePoznamky[poradiKnihy].Count}]</a></sup> ";
      }
      else if (cast is Poezie)
      {
        StringBuilder stavec = new StringBuilder();

        stavec.Append("<div class=\"poezie\">");

        foreach (CastTextu potomek in cast.Potomci)
        {
          stavec.Append(VygenerovatCastTextu(potomek, kniha, bible, dlouheCislaVerse));
        }

        stavec.Append("</div>");

        return stavec.ToString();
      }
      else if (cast is RadekPoezie)
      {
        StringBuilder stavec = new StringBuilder("<p>");

        foreach (CastTextu potomek in cast.Potomci)
        {
          stavec.Append(VygenerovatCastTextu(potomek, kniha, bible, dlouheCislaVerse));
        }

        stavec.Append("</p>");

        return stavec.ToString();
      }
      else if (cast is Odstavec)
      {
        StringBuilder stavec = new StringBuilder();

        stavec.Append("<p>");

        foreach (CastTextu potomek in cast.Potomci)
        {
          stavec.Append(VygenerovatCastTextu(potomek, kniha, bible, dlouheCislaVerse));
        }

        stavec.Append("</p>");

        return stavec.ToString();
      }
      else if (cast is FormatovaniTextu)
      {
        StringBuilder stavec = new StringBuilder();

        if ((cast as FormatovaniTextu).Kurziva)
        {
          stavec.Append("<i>");

          foreach (CastTextu potomek in cast.Potomci)
          {
            stavec.Append(VygenerovatCastTextu(potomek, kniha, bible, dlouheCislaVerse));
          }

          stavec.Append("</i>");
        }

        return stavec.ToString();
      }
      else if (cast is CastPoezie)
      {
        return $"<h5>{cast.TextovaData}</h5>\n";
      }
      else if (cast is CastTextuSTextem)
      {
        return cast.TextovaData;
      }
      else
      {
        StringBuilder stavec = new StringBuilder();

        foreach (CastTextu potomek in cast.Potomci)
        {
          stavec.Append(VygenerovatCastTextu(potomek, kniha, bible, dlouheCislaVerse));
        }

        return stavec.ToString();
      }
    }

    private static string ZiskatIdKapitoly(string puvodniIdKapitoly)
    {
      return $"kap-{puvodniIdKapitoly}";
    }

    private static string ZiskatIdKnihy(string puvodniIdKnihy)
    {
      return $"kni-{puvodniIdKnihy}";
    }

    private static string ZiskatKratkeCisloVerse(string id)
    {
      return id.Substring(id.LastIndexOf('.') + 1);
    }

    private static string ZiskatDlouheCisloVerse(Bible bible, string id)
    {
      string anglickaKniha = id.Substring(0, id.IndexOf('.'));
      string ceskaKniha = bible.MapovaniZkratekKnih[anglickaKniha].CeskaZkratka;

      id = id.Replace(anglickaKniha, ceskaKniha);

      StringBuilder bldr = new StringBuilder(id)
      {
        [id.IndexOf('.')] = ' ',
        [id.LastIndexOf('.')] = ','
      };

      return bldr.ToString();
    }

    public string VygenerovatEpub(Bible bible, bool dlouhaCislaVerse)
    {
      PouzitePoznamky.Clear();

      string pracovniAdresar = Environment.CurrentDirectory;
      string epubAdresar = Path.Combine(pracovniAdresar, bible.Metadata.Nazev);
      string epubSoubor = Path.Combine(pracovniAdresar, $"{bible.Metadata.Nazev}.epub");

      if (Directory.Exists(epubAdresar))
      {
        Utility.VymazatAdresar(epubAdresar);
      }

      Directory.CreateDirectory(epubAdresar);

      if (File.Exists(epubSoubor))
      {
        File.Delete(epubSoubor);
      }

      // Vygenerujeme úvodní texty, titulní obrázek,
      // úvodní nakladatelské informace, seznam knih.
      string obsahAdresar = Path.Combine(epubAdresar, "OEBPS");
      string metaAdresar = Path.Combine(epubAdresar, "META-INF");
      string htmlAdresar = Path.Combine(obsahAdresar, PodadresarHtml);
      string cssAdresar = Path.Combine(obsahAdresar, "css");
      string obrazkyAdresar = Path.Combine(obsahAdresar, "img");

      Directory.CreateDirectory(htmlAdresar);
      Directory.CreateDirectory(cssAdresar);
      Directory.CreateDirectory(metaAdresar);
      Directory.CreateDirectory(obrazkyAdresar);

      List<string> manifesty = new List<string>();
      List<string> spine = new List<string>();
      StringBuilder tocVnitrek = new StringBuilder();
      StringBuilder htmlPoznamek = new StringBuilder();

      foreach (Kniha kniha in bible.Knihy)
      {
        string nazevSouboruKnihy = ZiskatNazevSouboruKnihy(kniha);
        string souborKnihy = Path.Combine(htmlAdresar, nazevSouboruKnihy);
        string htmlMustr = Properties.Resources.kniha.Clone() as string;
        string htmlObsah = VygenerovatKnihu(kniha, bible, dlouhaCislaVerse);

        htmlObsah = $"<h1 id=\"{ZiskatIdKnihy(kniha.Id)}\">{bible.MapovaniZkratekKnih[kniha.Id].Nadpis}</h1>" + htmlObsah;

        htmlObsah = string.Format(
          htmlMustr ?? throw new InvalidOperationException(),
          bible.MapovaniZkratekKnih[kniha.Id].Nadpis,
          htmlObsah);

        // Vygenerujeme část TOC.
        List<UvodKapitoly> podkapitoly = kniha.ZiskatRekurzivniPotomky<UvodKapitoly>();

        if (podkapitoly.Any())
        {
          string podkapitolyVnitrek = string.Join(
            "\n",
            podkapitoly.Select(kap => $"<li><a href=\"{nazevSouboruKnihy}#{ZiskatIdKapitoly(kap.Id)}\">Kapitola {ZiskatKratkeCisloVerse(kap.Id)}</a></li>"));

          tocVnitrek.AppendLine(
            $"<li><a href=\"{nazevSouboruKnihy}\">" +
            $"{bible.MapovaniZkratekKnih[kniha.Id].Nadpis}</a><ol>{podkapitolyVnitrek}</ol></li>");
        }
        else
        {
          tocVnitrek.AppendLine($"<li><a href=\"{nazevSouboruKnihy}\">{bible.MapovaniZkratekKnih[kniha.Id].Nadpis}</a></li>");
        }

        // Přidáme soubor do manifestu a do páteře.
        manifesty.Add($"<item href=\"{PodadresarHtml}/{nazevSouboruKnihy}\" id=\"id-{nazevSouboruKnihy}\" media-type=\"application/xhtml+xml\"/>");
        spine.Add($"<itemref idref=\"id-{nazevSouboruKnihy}\"/>");

        File.WriteAllText(souborKnihy, htmlObsah, Encoding.UTF8);

        int poziceKnihy = bible.Knihy.IndexOf(kniha);

        // Zpracujeme poznámky z knihy.
        if (!PouzitePoznamky.ContainsKey(poziceKnihy))
        {
          continue;
        }

        // Nadpis sekce poznámek pro tuto knihu.
        htmlPoznamek.AppendLine($"<h2>{bible.MapovaniZkratekKnih[kniha.Id].Nadpis}</h2>");

        // Vlastní texty poznámek, každá poznámka je ve vlastním odstavci.
        htmlPoznamek.AppendLine(string.Join(
          "\n",
          PouzitePoznamky[poziceKnihy].Select((pozn, idx) => $"<p id=\"{ZiskatIdPoznamky(kniha, pozn)}\" epub:type=\"endnote\"><a epub:type=\"noteref\" href=\"{nazevSouboruKnihy}#{ZiskatIdCitace(kniha, pozn)}\">[{pozn.Id}]</a> {pozn.Text}</p>")));
      }

      string sumarPoznamek = string.Format(Properties.Resources.kniha_poznamky, htmlPoznamek);

      File.WriteAllText(Path.Combine(htmlAdresar, KnihaPoznamky), sumarPoznamek, Encoding.UTF8);

      // Kopírujeme a generujeme podpůrné soubory.
      File.WriteAllText(Path.Combine(cssAdresar, "kniha.css"), Properties.Resources.kniha_css);
      File.WriteAllBytes(Path.Combine(epubAdresar, "mimetype"), Properties.Resources.mimetype);
      File.WriteAllText(Path.Combine(metaAdresar, "container.xml"), Properties.Resources.container);
      Properties.Resources.cover.Save(Path.Combine(obrazkyAdresar, "cover.png"), ImageFormat.Png);
      Properties.Resources.logo.Save(Path.Combine(obrazkyAdresar, "logo.png"), ImageFormat.Png);

      // Nyní dynamické soubory.
      string obsahOpf = Encoding.UTF8.GetString(Properties.Resources.content);

      obsahOpf = string.Format(
        obsahOpf,
        bible.Metadata.Nazev,
        bible.Revize.Datum.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        string.Join("\n", manifesty),
        string.Join("\n", spine),
        $"{PodadresarHtml}/{ZiskatNazevSouboruKnihy(bible.Knihy.First())}",
        bible.MapovaniZkratekKnih[bible.Knihy.First().Id].Nadpis);

      File.WriteAllText(Path.Combine(obsahAdresar, "content.opf"), obsahOpf);

      // Generování TOC.
      tocVnitrek.Insert(0, $"<li><a href=\"kniha-uvod.html\">Úvod</a></li>");
      tocVnitrek.AppendLine($"<li><a href=\"kniha-toc.html\">Obsah</a></li>");

      string tocData = Properties.Resources.kniha_toc;

      tocData = string.Format(tocData, bible.Metadata.Nazev, tocVnitrek);
      File.WriteAllText(Path.Combine(htmlAdresar, "kniha-toc.html"), tocData, Encoding.UTF8);

      // Generování úvodního souboru.
      string uvodniSoubor = Properties.Resources.kniha_uvod;

      uvodniSoubor = string.Format(uvodniSoubor, bible.Metadata.Nazev, bible.Revize.Datum.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
      File.WriteAllText(Path.Combine(htmlAdresar, "kniha-uvod.html"), uvodniSoubor, Encoding.UTF8);
      File.WriteAllText(Path.Combine(htmlAdresar, "kniha-cover.html"), Properties.Resources.kniha_obalka, Encoding.UTF8);

      // Konverze zip -> epub.
      FastZip zip = new FastZip();

      FastZip z = new FastZip
      {
        CreateEmptyDirectories = true
      };

      z.CreateZip(
        $"{bible.Metadata.Nazev}-{bible.Revize.Datum.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}" +
        $"{(dlouhaCislaVerse ? "-l" : string.Empty)}.epub",
        epubAdresar, true, string.Empty);

      return epubSoubor;
    }

    private string ZiskatNazevSouboruKnihy(Kniha kniha)
    {
      return $"kniha-{kniha.Id}.html";
    }

    #endregion
  }
}