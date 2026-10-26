# The text: a critical read (2026-10-26)

The creator asked for every line to be read critically and rewritten. The story is the game's most important
ingredient. This is what's wrong, the rules for the rewrite, and where it stands.

**Status: the morning, the bar and the office are rewritten (2026-10-26, overnight, with the creator's go-ahead to carry on). Each file is its own commit, so any of it can be rolled back if the direction is wrong.**

## What's wrong

### 1. Every paragraph ends on a punchline
Setup, setup, zinger, in nearly every block of 16,700 words. It reads like stand-up, and it breaks two
requirements: no constant jokes, and moving naturally between comedy and tragedy. The sad moments get a quip straight
after, so they never land.
- Magda turns down a kiss: *"Sweet. Like a dog. Like a small, drunk dog."*
- Stojan stops talking to Teo: a beat that should hurt, closed with a gag about the No Smoking sign.

### 2. AI tells the project bans
- **"It's not X. It's Y."** At least eight times: *"It's not a yes. It's a map." "It's not brave. It's plum." "It's not
  a problem. It's a sentence that has started a lot of problems."*
- **A neat moral at the end of a line**: *"which is most of what anyone gets"*, *"Mostly nobody does. That's the problem
  with this country."*, *"Better is a long way from good, but they're on the same road."*
- **Tidy triplets for rhythm**: *"Glass, decking, little lights." "Leather. Cool air. A driver called Bogdan..."
  "Shower. Coffee. The good shirt..."*
- **Similes everywhere**: about 65 "like a ..." / "like I ..." in the script. Several scenes have one in every
  paragraph. One good image per scene is worth more.
- **Stock words**: "of course" 21 times, "genuinely" 7, "that's the worst part" again and again.

### 3. Everyone talks like Teo
Zdenka, Ramona, Viktor, Stojan, Laci and the hen party all deliver polished one-liners: *"I don't contribute to
weather." "There's a bag for every conference." "Viktor is a weather system."* When everyone is witty, nobody is a
person. Each one needs a voice of their own:

| Who | How they should sound |
|---|---|
| Teo | Inside: observant, dry, sometimes plain sad. Out loud: short, careful, says less than he thinks |
| Viktor | Short. Commands, first names, sports and boss phrases. Charm by volume, not wit |
| Magda | Fast, too many exclamation marks, jokes about herself before anyone else can. Breaks off mid-sentence |
| Zdenka | Facts and numbers. Few words. Never explains. The menace is in what she leaves out |
| Kornel | Formal, precise, over-explains, apologises for taking up time |
| Ramona | Polite and cold. One sharp line per scene at most, not five |
| Stojan | Confident broken English, big gestures, family as leverage |
| Laci, outsiders | Ordinary people at work. Mostly plain |

### 4. Teo always sounds the same
Drunk, hungover, scared, happy: the narration has one register. Drink and mood should change the sentences
themselves: shorter, looser, more repetition when drunk; flatter when low. The bar was the clearest case (the same
text every time); it's the sample.

### 5. Explaining the joke
A line lands, then the next line explains it: *"Gift for whom? We're the gift. We're giving ourselves five hundred
each."* Trust the player.

## Rules for the rewrite

1. At most one joke per paragraph, and some paragraphs with none. Let sad moments stay sad.
2. No "It's not X. It's Y.", no moral at the end, no triplets for rhythm. No em or en dashes.
3. One simile per scene, if it's earned.
4. Each person sounds like themselves (table above).
5. Drink, tiredness and mood change how Teo's sentences sound, not only what happens.
6. Keep what works: the creator-approved voice from `design/sample_scenes.md` (the ficus, "Viktor has met us, hasn't
   he?", "Nobody calls me that. Stojan calls me that.") and the story beats. No new events, no new characters.
7. Logic, flags and conditions stay identical. Tests and the simulator must pass after every file.

## Order

| # | File | Words | State |
|---|---|---|---|
| 1 | Morning: flat (`tuesday_home.ink`, `wake`) | 500 | **Sample done** (and the phone bug) |
| 2 | The bar (`tuesday_party.ink`, `party_drink`) | 400 | **Sample done**: changes with drink and with each round; tickets work |
| 3 | Stairwell, morning (`tuesday_home.ink`) | 400 | Two lines trimmed; the rest is creator-approved |
| 4 | The office (`tuesday_office.ink`) | 3,200 | **Done** (logic unchanged, checked line by line) |
| 5 | The party (`tuesday_party.ink`, `tuesday_people.ink`) | 5,800 | **Done** (logic unchanged) |
| 6 | The night (`tuesday_night.ink`) | 2,400 | To do |
| 7 | Wednesday (`wednesday.ink`) | 4,000 | To do |
| 8 | Teo's end-of-day lines and hindsight (`lib.ink`) | 900 | To do |
