using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Mtf.Maui.YouTube.OAuth2.Models;

public partial class Video : INotifyPropertyChanged
{
    private bool liked;
    private string newComment = String.Empty;

    public string Title { get; set; } = String.Empty;

    public string Description { get; set; } = String.Empty;

    public string VideoId { get; set; } = String.Empty;

    public string VideoType { get; set; } = String.Empty;

    public bool Liked
    {
        get => liked;
        set
        {
            if (liked == value)
            {
                return;
            }

            liked = value;
            OnPropertyChanged();
        }
    }

    private static readonly string[] comments =
    [
        "Köszönet a videóért JUHI!",
        "Nagyon hasznos tartalom!",
        "Mindig tanulok valamit tőled.",
        "Szuper videó volt, köszi!",
        "Ez tényleg elgondolkodtató.",
        "Remélem, lesz folytatás!",
        "Nagyon inspiráló volt, köszönöm!",
        "Ezt mindenkinek látnia kellene.",
        "Tök jó minőségű videó, profi munka!",
        "Sok sikert a továbbiakhoz!",
        "Ez pont most jött jól, köszi!",
        "Tartalmas és őszinte videó, gratula!",
        "Még, még, még, ennyi nem elég!",
        "Ez hatalmas segítség volt, köszi!",
        "Nagyon jól összefoglalod a lényeget.",
        "Most is tanultam valami újat.",
        "Pont ilyen tartalmakra van szükség!",
        "Nagyon jól magyarázol, respect!",
        "Ez a videó többször nézős!",
        "Mindig feldobnak a videóid!",
        "Tényleg értékes, amit csinálsz.",
        "Nagyon jó tempóban vezetted végig!",
        "Ezt mentem is későbbre!",
        "Egyszerűen szuper lett!",
        "Mindig öröm új videót látni tőled!",
        "Nem hittem, hogy ilyen jól értem majd, köszi!",
        "Nagyon profi előadás!",
        "Köszi a befektetett munkát!",
        "Nagyon jól összerakott tartalom, gratula!",
        "Ez most rengeteget segített, köszi!",
        "Mindig minőségi videókat hozol, respect!",
        "Könnyen követhető és érthető, szuper munka!",
        "Most is nagyot alkottál!",
        "Ez a magyarázat aranyat ér!",
        "Imádom, hogy ilyen érthetően adod elő!",
        "Ez a videó most életmentő volt!",
        "Alig várom a következőt!",
        "Nagyszerű stílusban magyarázol!",
        "Ez profi szint, semmi kétség!",
        "Ritkán látni ennyire tiszta összefoglalót!",
        "Most is bebizonyítottad, mennyire értesz hozzá!",
        "Nagyon jó példa volt, köszi!",
        "Végre valaki ilyen érthetően elmondja!",
        "Nagyon informatív és tömör, imádtam!",
        "Ezt tanítani kellene!",
        "Nagyon jól felépített magyarázat.",
        "Ez a videó nagyot szólt!",
        "Komolyan mondom, minden videód egyre jobb!",
        "Nagyon okos megközelítés!",
        "Ez most pont hiányzott, köszi!",
        "Ezt így még sosem értettem meg — most igen!",
        "Nagyon igényes tartalom!",
        "Ez tiszta, érthető és tökéletes volt!",
        "Köszi, hogy ennyi energiát teszel bele!"
    ];

    public string NewComment
    {
        get => newComment;
        set
        {
            if (newComment == value)
            {
                return;
            }

            newComment = value;
            OnPropertyChanged();
        }
    }

    public string Image { get; set; } = String.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public Video()
    {
        var index = RandomNumberGenerator.GetInt32(comments.Length);
        NewComment = comments[index];
    }
}