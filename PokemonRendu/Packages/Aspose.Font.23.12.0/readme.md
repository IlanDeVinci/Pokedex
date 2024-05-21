# Font Manipulation via .NET API

![Version 23.12.0](https://img.shields.io/badge/nuget-v23.12.0-blue) ![Nuget](https://img.shields.io/nuget/dt/Aspose.Font)

[![banner](https://raw.githubusercontent.com/Aspose/aspose.github.io/master/img/banners/aspose_font-for-net-banner.png)](https://releases.aspose.com/font/net/)

[Product Page](https://products.aspose.com/font/net/) | [Docs](https://docs.aspose.com/font/net/) | [API Reference](https://reference.aspose.com/font/net/) | [Examples](https://github.com/aspose-font/Aspose.Font-for-.NET/tree/master/Examples) | [Blog](https://blog.aspose.com/category/font/) | [Search](https://search.aspose.com/) | [Free Support](https://forum.aspose.com/c/font) | [Temporary License](https://purchase.aspose.com/temporary-license/)

[Aspose.Font for .NET](https://products.aspose.com/font/net/) is a library that enables your .NET applications to load, edit, convert, and save font data. Aspose.Font for .NET also enables your .NET applications to draw text with the specified font.

## Font Processing Features

- Load font files from the disc as well as stream.
- Read font information(name, family, etc., plus other string metadata) and save updated font files to disc.
- Convert fonts from one font format to another.
- Merge TrueType fonts.
- Create TrueType fonts from existing ones using the font merge feature.
- Render text using font glyphs.
- Render text using custom interfaces. 
- Supports ligatures and advanced OpenType features: glyph substitution (GSUB) and glyph positioning (GPOS) for rendering.
- Extract embedded licensing information from font files.
- Read Glyphs and Metrics information from Font files.
- Detect Latin Symbols in Fonts.

## Supported Formats

### Font Formats supported for reading and writing

TTF, WOFF/WOFF2

### Font Formats supported for reading

TTC, OpenType, CFF, Type1, EOT

### Font Formats supported for writing

SVG

## Platform Independence

Aspose.Font for .NET can be integrated with any kind of ASP.NET Web Application or a Windows Application.

## Getting Started

Are you ready to give Aspose.Font for .NET a try? Simply execute `Install-Package Aspose.Font` from Package Manager Console in Visual Studio to fetch the NuGet package. If you already have Aspose.Font for .NET and want to upgrade the version, please execute `Update-Package Aspose.Font` to get the latest version.

## Loading of font with TrueType format (.ttf)
```csharp
using Aspose.Font;
using Aspose.Font.Sources;

// Font file name with the full path
string fontPath = Path.Combine(DataDir, "Montserrat-Regular.ttf");

// Initialize FontFileDefinition based on FileSystemStreamSource object, set fileExtension to "ttf" 
FontFileDefinition fileDef = new FontFileDefinition("ttf", new FileSystemStreamSource(fontPath));

// Initialize FontDefinition using just created FontFileDefinition object, passing TTF as the FontType value
FontDefinition fontDef = new FontDefinition(FontType.TTF, fileDef);

// Load the font
Font font = Font.Open(fontDef);
```

## Extracting font name, font family
```csharp
using Aspose.Font;

//Reference to font object loaded from some source
Font font;
string fontName = font.FontName;
string fontFamily = font.FontFamily;
```

## Convert TTF to WOFF2
```csharp
using Aspose.Font.Sources;
using System.IO;

// Open TTF font
string fontPath = Path.Combine(DataDir, "Montserrat-Regular.ttf");
FontDefinition fontDefinition = new FontDefinition(FontType.TTF, new FontFileDefinition(new FileSystemStreamSource(fontPath)));
Font font = Font.Open(fontDefinition);

// Write the output settings for the WOFF2 format
string outPath = Path.Combine(OutputDir, "out.woff2");
FileStream outStream = File.Create(outPath);

// Make conversion from TTF to WOFF2 and save the result using SaveToFormat() method of the base abstract Font class instance
font.SaveToFormat(outStream, FontSavingFormats.WOFF2);
```

## Render text
```csharp
using System.IO;
using Aspose.Font;
using Aspose.Font.Renderers;

//Reference to font object loaded from some source
Font font;
//Text to render using the specified font
string text = "Hello world";
//Set font size to 12
using (Stream st = RenderingUtils.DrawText(font, text, 12))
{
	//Save rendered text to the file
	byte[] buff = new byte[st.Length];
	st.Read(buff);	
	File.WriteAllBytes("Text.png", buff);
}                        

```

## Generate TrueType font
```csharp
using Aspose.Font.Ttf;
using Aspose.Font.TtfHelpers;

// Original font to take glyphs from
TtfFont srcFont;

//To create a font we can use functionality of the IFontCharactersMerger interface.
//The IFontCharactersMerger.MergeFonts() method is designed to create a font as a result of merging two fonts.
//It takes two arrays of glyphs or character codes and puts into resultant font only glyphs from these arrays.
//But we can use a trick by passing two references on same font and provide only single array of character codes.

IFontCharactersMerger merger = HelpersFactory.GetFontCharactersMerger(srcFont, srcFont);

TtfFont destFont = merger.MergeFonts(new uint[] { 'a', 'c', 'e' }, new uint[0], "TestFont");

//Save resultant font
destFont.Save("TestFont.ttf");

```
[Product Page](https://products.aspose.com/font/net/) | [Docs](https://docs.aspose.com/font/net/) | [API Reference](https://reference.aspose.com/font/net/) | [Examples](https://github.com/aspose-font/Aspose.Font-for-.NET/tree/master/Examples) | [Blog](https://blog.aspose.com/category/font/) | [Search](https://search.aspose.com/) | [Free Support](https://forum.aspose.com/c/font) | [Temporary License](https://purchase.aspose.com/temporary-license/)
