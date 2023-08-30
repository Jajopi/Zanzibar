# Programová dokumentácia

Program je vytvorený vo vývojovom prostredí Unity,
pričom využíva pre časť svojej funkcionality vlastnosti tohto prostredia
a pre zbytok "scripty" -- triedy napísané v jazyku C#
(v priečinku Scripts).

## Scény

Program obsahuje dve scény.
Prvá tvorí hlavné menu programu, druhá samotnú hru.

### MenuScene

V tejto scéne sa nachádza kamera a klasický UI objekt v Unity -- Canvas,
ktorý dovoľuje zobrazovať nadpisy, tlačidlá a políčka na zadávanie textu.

Túto scénu ovláda script MenuScript, ktorý sa stará o akcie tlačidiel,
prepínanie do scény s hrou, a pokiaľ má byť zahájená nová hra,
tak aj zmaže uložené údaje a uloží namiesto nich informáciu o tom,
akí hráči budú v novej hre hrať.

### GameScene

Táto scéna zabezpečuje beh samotnej hry. Väčšinu hry ovláda objekt Board,
ktorý má dva jedinečné skripty Board a Logic.

Ďalej sa v scéne nachádza kamera ovládaná scriptom CameraMovement
a pohybovateľná klávesmi, a tiež objekt Canvas, ktorý ovláda prvky
zobrazované do UI pomocou scriptu Quests.

Okrem toho sa v scéne nachádza väčšie množstvo
objektov typu Node, Figure a Quest, čo sú generické objekty s danými
scriptami, ktoré ich ovládajú.

#### Scripty

Script Board má na starosti vytvorenie plochy z reprezentácie v
textovom súbore, pomocou objektov typu Node a čiar medzi nimi.
Zabezpečuje tiež počiatočné umiestnenie figurek (objektov typu Figure).

Script Logic je zase zodpovedný za fungovanie jednotlivých ťahov,
ich valídnosť, časovanie kôl, počítanie bodov a čiastočne aj
vyhodnocovanie bodovaných úloh.
Vyhodnocuje teda v konečnom dôsledku aj klikanie do scény.

Ďalej sa Logic stará o ukladanie a opätovné načítanie
údajov o hre, ako sú polohy jednotlivých
figúrok, aktuálny hráč, body hráčov, úlohy každého hráča.
Využíva na to jednoduchý systém PlayerPrefs, ktorý umožňuje ukladať len
čísla alebo reťazce, a to bez akéhokoľvek šifrovania, preto je možné ich
upravovať, a teda ideálne by bolo použiť iný systém, v kontexte
použitia tejto hry to však nebolo potrebné.

V každom bode v čase sa kolo môže nachádzať v jednom z 5 stavov (states).
Tie sú spravované scriptom Logic a sú nasledujúce:

- čakanie na zvolenie vlastnej figúrky
- čakanie na zvolenie miesta, kam sa má pohnúť
- čakanie na zvolenie miesta, kam odsunúť cudziu figúrku
- alebo vyhodiť cudziu figúrku (ak nastane možnosť podľa pravidiel)
- čakanie na koniec ťahu (keď hráč vyčerpal pohyby)

Scripty Node a Figure udržujú informácie o políčkach a figurkách,
ako sú susedia políčok, figurky políčok, políčka figuriek,
hráči a čísla figuriek, ale aj zachytávajú a posúvajú ďalej udalosti
spôsobené kliknutím na dané políčko / figúrku.

Script Quests vypisuje na Canvas všetky potrebné informácie,
ako hráčov a ich aktuálne body, aktuálneho hráča a jeho zostávajúci čas,
jednotlivé úlohy, keď ich hráči chcú vidieť, a tiež rieši existenciu
týchto úloh: ich načítanie z textového súboru na začiatku,
vytváranie objektov typu Quest, vymieňanie úloh a časť vyhodnocovania
úloh (pošle údaje scriptu Logic).

Script Quest si pamätá údaje o konkrétnej úlohe, ako je hráč,
ktorému patrí, jednotlivé počty bodov v závislosti od úrovne splnenia,
zadanie, ktoré sa má pre konkrétnu úlohu plniť.

Okrem toho existuje ešte script LightScript, ktorý ovláda dva objekty
typu Directional light, otáča ich a vymieňa, a tým zabezpečuje vizuálne
striedanie dňa a noci v intervale dĺžky jedného ťahu
(pokiaľ hráč ťah ukončí predčasne, nebude ráno vychádzať presne na
začiatok nového ťahu -- v pôvodnej implementácii nešlo ťahy preskakovať).

Script NumberRotation otáča čísla figúrok tak, aby boli v každom
momente viditeľné -- teda priamo do kamery.
Figúrka teda obsahuje aj objekt s číslom.

Script Pair som používal najmä na začiatku na ukladanie súradníc,
jeho využitie je ale len v scripte Board. Umožňuje vyrábať
páry akýchkoľvek typov hodnôt.
