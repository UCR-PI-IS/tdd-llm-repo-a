using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.ValueObjects.UniversityManagement;

namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Presentation.Helpers;
public static class ColorTranslator
{
    public static readonly List<string> AllColors = new()
    {
        "Red", "Green", "Blue", "Yellow", "Black", "White", "Orange", "Purple",
        "Gray", "Brown", "Pink", "Cyan", "Magenta", "Lime", "Teal", "Olive",
        "Navy", "Maroon", "Silver", "Gold"
    };

    public static List<Colors> GetAvailableColors() =>
        AllColors.Select(c => Colors.Create(c)).ToList();

    public static string ToSpanish(string color) => color switch
    {
        "Red" => "Rojo",
        "Green" => "Verde",
        "Blue" => "Azul",
        "Yellow" => "Amarillo",
        "Black" => "Negro",
        "White" => "Blanco",
        "Orange" => "Naranja",
        "Purple" => "Morado",
        "Gray" => "Gris",
        "Brown" => "Marrón",
        "Pink" => "Rosa",
        "Cyan" => "Cian",
        "Magenta" => "Magenta",
        "Lime" => "Lima",
        "Teal" => "Verde Azulado",
        "Olive" => "Oliva",
        "Navy" => "Azul Marino",
        "Maroon" => "Granate",
        "Silver" => "Plateado",
        "Gold" => "Dorado",
        _ => color
    };
}
