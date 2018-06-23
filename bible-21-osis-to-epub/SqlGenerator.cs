using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using BibleDoEpubu.ObjektovyModel;

namespace BibleDoEpubu
{
  internal class SqlGenerator
  {
    #region Vlastnosti

    private string AktualniTextVerse
    {
      get;
      set;
    }

    private List<string> Nadpisy
    {
      get;
      set;
    } = new List<string>();

    private int PocitadloKapitol
    {
      get;
      set;
    }

    private int PocitadloNadpisu
    {
      get;
      set;
    }

    private int GlobalniPocitadloVersu
    {
      get;
      set;
    }

    private int PocitadloVerse
    {
      get;
      set;
    }

    private int PoradiKnihy
    {
      get;
      set;
    }

    private List<PouzitaPoznamka> PouzitePoznamky
    {
      get;
      set;
    } = new List<PouzitaPoznamka>();

    private StringBuilder StavecKnihy
    {
      get;
    } = new StringBuilder();

    private StringBuilder StavecNadpisy
    {
      get;
    } = new StringBuilder();

    private StringBuilder StavecVerse
    {
      get;
    } = new StringBuilder();

    private List<string> Verse
    {
      get;
      set;
    } = new List<string>();

    #endregion

    #region Metody

    public string VygenerovatSql(Bible bible)
    {
      PocitadloNadpisu = 1;

      for (int poradi = 0; poradi < bible.Knihy.Count; poradi++)
      {
        Kniha kniha = bible.Knihy[poradi];

        PoradiKnihy = poradi + 1;
        PocitadloKapitol = 0;
        PocitadloVerse = 0;
        AktualniTextVerse = string.Empty;
        Nadpisy.Clear();
        Verse.Clear();

        StavecKnihy.Append("INSERT INTO bible_knihy (id, kod, nazev, `order`) VALUES " +
                           $"({poradi + 1}, '{kniha.Id}', '{bible.MapovaniZkratekKnih[kniha.Id].Nadpis}', {poradi + 1});\n");

        VygenerovatSqlProKnihu(bible, kniha);

        StavecNadpisy.Append(string.Join(string.Empty, Nadpisy));
        StavecNadpisy.AppendLine();

        StavecVerse.Append("INSERT INTO bible_verse (kniha_id, kapitola, vers, text, stripped, `order`) VALUES \n");
        StavecVerse.Append(string.Join(",\n", Verse));
        StavecVerse.Append(";");
      }

      string pracovniAdresar = Environment.CurrentDirectory;
      string sqlSoubor = Path.Combine(pracovniAdresar, $"{bible.Metadata.Nazev}.sql");
      Encoding kodovani = new UTF8Encoding(false);

      File.WriteAllText(sqlSoubor, string.Empty, kodovani);
      File.AppendAllText(sqlSoubor, StavecKnihy.ToString(), kodovani);
      File.AppendAllText(sqlSoubor, StavecNadpisy.ToString(), kodovani);
      File.AppendAllText(sqlSoubor, StavecVerse.ToString(), kodovani);

      return sqlSoubor;
    }

    public void VygenerovatSqlProKnihu(Bible bible, Kniha kniha)
    {
      VygenerovatCastSql(kniha, bible, kniha);
    }

    public void VygenerovatCastSql(CastTextu cast, Bible bible, Kniha kniha)
    {
      switch (cast.GetType().ToString()){
        case "HlavniCastKnihy":
          PridatRozpracovanyVers();
          goto case "CastKnihy";

        case "CastKnihy":
          VygenerovatCastKnihy(cast, bible, kniha);
          break;

        case "UvodKapitoly":
          PridatRozpracovanyVers();
          PocitadloVerse = 1;
          PocitadloKapitol++;
          break;

        case "Vers":
          PridatRozpracovanyVers();
          break;

        case "Poezie":
          VygenerovatCastPoezie(cast, bible, kniha);
          break;

        case "RadekPoezie":
          VygenerovatRadekPoezie(cast, bible, kniha);
          break;

        case "Odstavec":
          VygenerovatOdstavec(cast, bible, kniha);
          break;
        
        case "FormatovaniTextu":
          VygenerovatFormatovaniTextu(cast, bible, kniha);
          break;
        
        case "CastPoezie":
          AktualniTextVerse += $"<h5>{cast.TextovaData}</h5>\n";
          break;

        case "CastTextuSTextem":
          AktualniTextVerse += cast.TextovaData;
          break;

        case "Kniha":
          VygenerovatKnihu(cast, bible, kniha);
          break;
      }
    }
    private void VygenerovatCastKnihy(CastTextu cast, Bible bible, Kniha kniha)
    {
      VlozitSqlNadpis(cast is HlavniCastKnihy knihy ? knihy.Nadpis : ((CastKnihy) cast).Nadpis);

      foreach (CastTextu potomek in cast.Potomci)
      {
        VygenerovatCastSql(potomek, bible, kniha);
      }
    }

    private void VygenerovatCastPoezie(CastTextu cast, Bible bible, Kniha kniha)
    {
      if (!string.IsNullOrEmpty(AktualniTextVerse)) {
        AktualniTextVerse += "<br/>";
      }
      foreach (CastTextu potomek in cast.Potomci)
      {
        VygenerovatCastSql(potomek, bible, kniha);
      }
    }

    private void VygenerovatRadekPoezie(CastTextu cast, Bible bible, Kniha kniha)
    {
      foreach (CastTextu potomek in cast.Potomci)
      {
        VygenerovatCastSql(potomek, bible, kniha);
      }

      AktualniTextVerse += "<br/>";
    }

    private void VygenerovatOdstavec(CastTextu cast, Bible bible, Kniha kniha)
    {
      AktualniTextVerse += "<br/>";
      foreach (CastTextu potomek in cast.Potomci)
      {
        VygenerovatCastSql(potomek, bible, kniha);
      }
    }

    private void VygenerovatFormatovaniTextu(CastTextu cast, Bible bible, Kniha kniha)
    {
      if ((cast as FormatovaniTextu).Kurziva)
      {
        AktualniTextVerse += "<i>";

        foreach (CastTextu potomek in cast.Potomci)
        {
          VygenerovatCastSql(potomek, bible, kniha);
        }

        AktualniTextVerse += "</i>";
      }
    }

    private void VygenerovatKnihu(CastTextu cast, Bible bible, Kniha kniha)
    {
      foreach (CastTextu potomek in cast.Potomci)
      {
        VygenerovatCastSql(potomek, bible, kniha);

        PridatRozpracovanyVers();
      }
    }

    private void PridatRozpracovanyVers()
    {
      if (!string.IsNullOrEmpty(AktualniTextVerse) && AktualniTextVerse != "<br/>")
      {
        Verse.Add($"({PoradiKnihy}, '{PocitadloKapitol}', '{PocitadloVerse}', '{RemoveMultipleSpaces(AktualniTextVerse)}', " +
                  $"'{OstripovatVers(AktualniTextVerse)}', {GlobalniPocitadloVersu++})");
        PocitadloVerse++;
      }

      AktualniTextVerse = string.Empty;
    }

    private object OstripovatVers(string aktualniTextVerse)
    {
      byte[] tempBytes;
      tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(aktualniTextVerse);
      string strippedStr = Encoding.UTF8.GetString(tempBytes);
      strippedStr = strippedStr.Replace("\"", string.Empty);
      strippedStr = strippedStr.Replace("'", string.Empty);
      strippedStr = Regex.Replace(strippedStr, @"<.*?>|\t|\n|\r", string.Empty);
      strippedStr = Regex.Replace(strippedStr, @"\s\s+", " ");

      return strippedStr;
    }

    private object RemoveMultipleSpaces(string aktualniTextVerse)
    {
      String strippedStr = Regex.Replace(aktualniTextVerse, @"\s\s+", " ");
      return strippedStr;
    }

    private void VlozitSqlNadpis(string nadpis)
    {
      Nadpisy.Add($"INSERT INTO bible_nadpisy (id, kniha_id, kapitola, vers, text, offset) " +
                  $"VALUES({PocitadloNadpisu}, {PoradiKnihy}, '{PocitadloKapitol}', '{PocitadloVerse}', '{nadpis}', 0);\n");

      PocitadloNadpisu++;
    }

    #endregion
  }
}