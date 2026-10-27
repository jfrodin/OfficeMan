# The Misadventures of Office Man: the script

Every line in the game, in the order a day can run. Written out from `content/ink` (`dotnet run --project tools/OfficeMan.Script`); the ink is still the source, so changes made here are carried back into it by hand.

How to read it:

- Plain paragraphs are Teo's own voice: what he sees and thinks. **NAME:** is someone speaking.
- **▸ Bold with an arrow** is a choice the player makes. Tags after a choice say what kind it is: *[bold]*, *[cautious]*, *[kind]*, *[drink]*.
- *If ...:* and *Otherwise:* are variations. Only one of them plays. What follows each is indented under it.
- Teo's state: drunk runs sober, tipsy, drunk, very drunk, wrecked. Mood runs from low (-2) to good (+2). His traits are lines with two ends: nerve (bold or careful), warmth (open or closed), appetite (hungry or held back), honesty (blunt or smooth). Regard is what someone thinks of Teo, -10 to 10.
- A *hidden check* uses one of Teo's traits plus luck, and it goes badly, half well or well. The player never sees a number.
- Names in `backticks` are things the game remembers (things Teo did or found out). The italic line under each heading says when a scene can happen, and its name in the files.

## TUESDAY 06:47. TEO'S FLAT

### Wake

*`wake`*

*(Note: What Teo has until payday on Friday. A first guess; the creator decides pay and payday.)*

*[Teo has 70 euro.]*

- *If Teo is in a good mood (mood 1 or more):*

  I wake up before the alarm and I feel fine. Rested, even.

  Something's going to go wrong today, and I'm going to deserve it.

- *Or if Teo is in a low mood (mood -2 or less):*

  The alarm goes off. I don't move.

  There's a crack in the ceiling shaped like Italy, or a leg. Six years I've lived here and I still haven't decided.

- *Otherwise:*

  The alarm goes off. I've been awake for twenty minutes, waiting for it.

Tuesday. The summer party. Viktor put it on a Tuesday so nobody would drink.

Viktor has met us, hasn't he?

**STOJAN:** *(through the wall)* Because it's MY balcony!

A door slams. Then the daughter's music through the wall: bass, and a girl singing in English about being disappointed.

- ▸ **Get up.** *→ get up*

- ▸ **Check phone.** *(only if not `phone`)* *→ phone*

- ▸ **Check phone again.** *(only if `phone` and not `phone_again`)* *→ phone again*

- ▸ **Lie here a bit longer.** *(only if not `lie`)* *→ lie*

#### phone

Two messages.

Magda, to the office group: *PARTY TONITE!!! dont forget gift money 50 each!!! see u all xx*

Gift for who? Nobody's leaving. Nobody's getting married. Magda will have a reason by lunch.

And one from the bank, which I didn't ask for: *Balance: 70.00 EUR*. Payday is Friday.

Kornel, just to me: *Good morning Teo. Would you like to share a taxi to the venue? I have calculated that it is cheaper.*

He's calculated it. There's a spreadsheet somewhere with my name on it.

- ▸ **Reply: "Sure."**

  The answer comes in four seconds. He was holding the phone.

  *Excellent. I will send you the cost breakdown.*

- ▸ **Reply: "Might go straight from the office."**

  *Understood. The offer remains open.*

- ▸ **Don't reply.**

  I put the phone face down. He'll have seen the two blue ticks by now.

*→ back to the choices*

#### lie

Thirty-two. When my father was thirty-two he had a wife, two kids and a Lada.

I have a ficus.

The ficus is dying. We understand each other.

*→ back to the choices*

#### phone again

- *If `kornel_taxi` is 1:*

  One new message from Kornel. *Cost breakdown attached.*

  It's a PDF. He's put a logo on it.

- *Otherwise:*

  Nothing new. It's just later than it was.

*→ back to the choices*

#### get up

- *If the first time and Teo is in a low mood (mood less than 0):*

  I get up the way old men get out of the sea.

Shower. Coffee. I put on the good shirt for the party, then take it off again. The good shirt has a story on it.

## TUESDAY 07:35. THE STAIRWELL

### Stojan.

*Teo's choice, at: stojan · takes 10 min · `stairwell_stojan`*

*(Note: The landing is a room Teo walks through. Stojan calls out when he comes near; going over to him is Teo's choice. Walking on past him is the other one (stairwell_down).)*

Stojan is on the landing in a vest, smoking under the sign that says *No Smoking in the Stairwell*. He put the sign up.

**STOJAN:** Teo! Mr Permit!

Nobody calls me that. Stojan calls me that.

- *If nerve 0 or more:*

  I could tell him no. I won't. But I could.

- *Or if Teo is careful (nerve -2 or less):*

  I already know I'm going to say yes to whatever this is.

**STOJAN:** Listen. Small thing. The balcony.

The "balcony" is a room now. It has a window, a roof and, since May, a toilet.

**STOJAN:** People from your office came. Took pictures. Very rude. Very professional. You make this go away, yes? Neighbours.

- ▸ **"I can't really do that, Stojan."** *[cautious]* *→ refuse*

- ▸ **"Who came? Do you know their names?"** *→ names*

- ▸ **"I'll see what I can do."** *[kind]* *→ promise*

- ▸ **"What's in it for me?"** *[bold] (only if mood 0 or more or Teo is tipsy or worse or nerve 0 or more)* *→ bargain*

#### refuse

**STOJAN:** Of course. Of course. Rules.

He nods as if I've said something wise. Then he looks at my door for slightly too long, like a man memorising a door.

#### names

**STOJAN:** A woman. Older. Grey suit. She measured everything twice and said nothing. Not even good morning.

Zdenka. She measures everything twice. Some things three times, and nobody knows which.

**STOJAN:** You know her?

- ▸ **"She's good. She'll be fair."**

  **STOJAN:** Fair. Fair is for football.

- ▸ **"I'll have a word."** *→ promise*

He flicks ash at the sign.

#### promise

In this country that sentence has never once meant yes. Stojan knows that as well as I do.

He smiles anyway. He's decided to hear it.

**STOJAN:** Good man. I knew. I said to Dana, Teo is a good man. Quiet, but good.

Through the wall, Dana says something that doesn't sound like agreement.

#### bargain

Stojan laughs, a big one. He points his cigarette at the wall.

**STOJAN:** You fix this, you can marry my daughter.

**TEO:** She's seventeen, Stojan.

**STOJAN:** So you have one year to fix it. I'm a patient man.

Through the wall, something breaks. Stojan doesn't flinch.

He's a patient man.

### Go down to work.

*A way out · takes 5 min · `stairwell_down`*

- *If `stairwell_stojan` hasn't happened:*

  **STOJAN:** *(behind me)* Later, Mr Permit! Later we talk!

  I take the stairs two at a time, like a man with somewhere to be.

- *Otherwise:*

  I take the stairs down. Four floors. The lift has been "nearly fixed" since March.

## TUESDAY 08:30. CITY PLANNING OFFICE, PERMITS SECTION

### Office arrive

*On arrival · `office_arrive`*

The Permits Section is on the fourth floor of a building that was refused a permit in 1974 and built anyway.

Eleven desks, two printers that work, and a poster from the EU Cohesion Fund: a smiling family on a bridge we haven't built yet.

Zdenka has been here since six. Zdenka is always here since six.

Magda has brought balloons. Nobody asked for balloons.

Kornel waves at me like we're on opposite sides of an airport.

### Viktor speech

*09:10, happens by itself · takes 10 min · `viktor_speech`*

Viktor comes out of his office and claps once. He's never needed to clap twice.

- *If honesty 0 or more:*

  One day I'll tell him that clap is ridiculous. Maybe today.

- *Or if Teo is smooth (honesty -2 or less):*

  I'm already smiling. I didn't decide to. My face just does that for Viktor.

He's fifty, he's tanned in a way nobody in this building can afford, and his shirt is open one button further than the dress code allows. He wrote the dress code.

**VIKTOR:** People! Tonight. Terasa Velka, five o'clock. The city's paying, so behave.

**VIKTOR:** It's a Tuesday. Two drink tickets each. We're professionals.

Magda laughs as if it was a joke. Zdenka writes something down.

- ▸ **Laugh along.**

  I laugh, a second late. Viktor points at me anyway. Any laugh will do.

- ▸ **Say nothing.**

  I say nothing. Nobody notices that either.

- ▸ **Make a joke.** *[bold]*

  *(A hidden check on Teo's nerve.)*

  **TEO:** Two tickets. So Magda's got thirty.

  - *If it goes badly:*

    Nobody laughs. Then Magda does, a second later and louder than anyone, and that's worse.

    Viktor looks at me as if I've parked in his space.

    **VIKTOR:** *(displeased)* Let's be nice, Teo. It's a party.

    He's right, and he said it in front of everyone.

  - *If it goes half well:*

    A few people laugh. Viktor, mostly. Magda smiles at the floor.

    **VIKTOR:** Thirty-two! She's been collecting since March!

    Magda laughs too. Then she goes quiet and straightens a balloon that was already straight.

  - *If it goes well:*

    It lands. Even Magda laughs, properly, and fans the tickets out of her cardigan pocket like a winning hand.

    **MAGDA:** Thirty-four, thank you very much!

    **VIKTOR:** That's my department! Look at that!

    For a moment I'm someone who says things. I don't know what to do with it.

Viktor goes back into his office and leaves the door open, so we can all see him being busy.

### Viktor office

*11:30, happens by itself · takes 20 min · `viktor_office`*

**VIKTOR:** Teo. In here a second.

Viktor's office has a view of the river and a framed photo of Viktor shaking hands with a minister who's now in prison. He keeps it up. He says it's history.

He sits on the edge of the desk, not behind it. He read that somewhere. It works.

**VIKTOR:** You're good, you know that? Quiet. But good. Zdenka says you're the only one who reads the attachments.

- *If Viktor remembers: funny:*

  **VIKTOR:** And funny! Who knew?

**VIKTOR:** Listen. Small thing. Tonight.

He lowers his voice. He never lowers his voice, so it isn't a small thing.

**VIKTOR:** Magda. Lovely girl. Lovely. But after a few drinks she gets... ideas. About me. You know.

I know. Everyone knows. The pigeons on the ledge know.

**VIKTOR:** And I might need to talk to some people tonight. Business. So if you could just be around her. Keep her happy. You're good with her.

**VIKTOR:** You'd be doing me a favour. I remember favours, Teo.

- ▸ **Agree.**

  **VIKTOR:** My man.

  He does something with his hand, as if he's going to pat my face, and pats my shoulder instead. I'm almost disappointed.

  I've just agreed to keep Magda busy so a married man can go after someone else. I didn't even pause.

- ▸ **Refuse. "That's not my job, Viktor."** *[cautious]*

  **VIKTOR:** No, no. Of course. Forget it.

  He's still smiling when I leave. He can smile and write your name in a small book at the same time.

- ▸ **Ask who he's "talking business" with.** *[bold]*

  *(A hidden check on Teo's nerve.)*

  - *If it goes well:*

    Viktor laughs and points at me.

    **VIKTOR:** See? Funny!

    He doesn't answer. Then he winks.

  - *Otherwise:*

    **VIKTOR:** Business, Teo. Grown-up business.

    The room goes a degree colder.

    **VIKTOR:** Forget I asked. Go on.

### Magda is collecting the gift money.

*Teo's choice, at: magda · open 08:30 to 14:30 · takes 15 min · `magda_gift`*

Magda's desk is ready for the party: balloons, a clipboard, and a tin that says *GIFT* in glitter glue.

**MAGDA:** Teo! You're the last one. Well. You and Zdenka, and Zdenka said no in a way I didn't like.

- *If Magda remembers: joked about her:*

  She smiles the same as always. It just stops at her mouth.

**TEO:** What's the gift for?

**MAGDA:** For Viktor! To say thank you. For the party.

We're buying Viktor a present to thank him for a party our taxes are paying for.

- ▸ **Put in fifty.** *[kind]*

  *[Money -50 euro]*

  **MAGDA:** *(happy)* You're a sweetheart! I'm getting him a pen. A proper one. Heavy.

  He'll love it. He'll sign things with it that she isn't allowed to read.

  That leaves me twenty euro until Friday. I don't say so.

- ▸ **Put in twenty.**

  *[Money -20 euro]*

  She looks at the note and doesn't say anything. She writes *Teo, 20* on the clipboard, very small and very neat.

- ▸ **Say you'll bring it later.** *[cautious]*

  **MAGDA:** Course. Later.

  She puts a little dot next to my name. Zdenka has a dot too.

She lowers her voice.

**MAGDA:** Can I show you something? Be honest.

She turns her phone round. A red dress on a hanger, in a fitting room, photographed from above like evidence.

**MAGDA:** For tonight. Do you think Viktor likes red?

- ▸ **Be kind. "You'll look great."** *[kind]*

  **MAGDA:** You think?

  **TEO:** I think.

  She holds the phone against her chest for a second.

  I didn't answer her question. She didn't notice.

- ▸ **Be honest. "I don't think it's about the dress, Magda."**

  *(A hidden check on Teo's tact.)*

  - *If it goes badly:*

    **MAGDA:** *(sad)* Oh. Okay. No, okay.

    She puts the phone away, face down. Then she laughs at nothing, so it looks like she's fine.

  - *Otherwise:*

    She looks at me for a long time.

    **MAGDA:** I know. I know it's not.

    **MAGDA:** I'm wearing it anyway.

    Good for her. I mean that.

- ▸ **Make it a joke. "He likes anything with a pulse."** *[bold]*

  **MAGDA:** Teo!

  She slaps my arm, delighted. She's taken it as good news.

  Maybe it is, for her, tonight. I don't know.

### Go and see Zdenka.

*Teo's choice, at: zdenka · takes 15 min · `zdenka_file`*

Zdenka's desk has nothing on it but one file, one pen and a metal ruler. I've never seen her measure anything with the ruler.

She doesn't look up. She knows who it is from the footsteps.

**ZDENKA:** Teo.

- *If `zdenka_dark` and a 50% chance:*

  She's sharpening a pencil with a small folding knife. Slowly. The shaving comes off in one long curl.

  She sees me watching.

  **ZDENKA:** Sharpeners are for people in a hurry.

- *If `stojan_promise` or `stojan_asked`:*

  *→ petrovici*

- *Otherwise:*

  *→ small talk*

#### small talk

**TEO:** Morning, Zdenka. Coming tonight?

**ZDENKA:** I'm going. That's not the same as coming.

- ▸ **Ask her why she didn't pay for the gift.**

  **ZDENKA:** A man who earns three times my salary is getting a pen from a woman who can't afford her dentist.

  That's the longest thing she's ever said to me. I don't think it was meant for me.

- ▸ **Leave her to it.**

  **ZDENKA:** Mm.

#### petrovici

**TEO:** The Petrovici inspection. The balcony.

**ZDENKA:** It is not a balcony.

**TEO:** No.

**ZDENKA:** It is a fourteen-square-metre unpermitted dwelling extension with plumbing, cantilevered over a public footpath on two steel brackets designed for air conditioning units.

**ZDENKA:** If it falls, it falls on a bus stop.

She says it the way other people say *looks like rain*.

- ▸ **Ask what happens next.**

  **ZDENKA:** I file it Thursday. Then a letter. Then a fine. He ignores the letter. Then a second letter. Then, in a year, someone dies or nobody does.

  **ZDENKA:** Mostly nobody does.

- ▸ **Ask if it could... take a while to file.** *[bold] (only if `stojan_promise`)*

  *(A hidden check on Teo's tact.)*

  She looks up. Her eyes are very pale, like winter.

  - *If it goes badly:*

    **ZDENKA:** No.

    Just that. She goes back to her file, and I'm left standing at her desk like a coat stand.

    I'll be thinking about that no at three in the morning.

  - *Otherwise:*

    **ZDENKA:** He's your neighbour.

    **TEO:** He's my neighbour.

    **ZDENKA:** Then he's your problem, not my file. The file is on the trolley. I file Thursday.

    That isn't a yes. She's told me where the file is, and when.

- ▸ **Leave it.**

  **ZDENKA:** Mm.

### Find the Petrovici file on the trolley.

*Teo's choice, at: trolley · only if `knows_file` and `stojan_promise` · takes 20 min · `petrovici_file`*

The trolley is where files wait. Some of them wait for years.

*PETROVICI, S. Unauthorised extension. Inspection: Z. Horák.*

There are photos. The "balcony" from below. It looks like a shed trying to escape.

And there's the bus stop.

- ▸ **Put it at the bottom of the trolley. Thursday becomes next month.** *[bold]*

  *(A hidden check on Teo's judgment.)*

  *[Headline: Teo buried the Petrovici file.]*

  I slide it under eleven other files. It's the most decisive thing I've done this year.

  - *If the check goes badly:*

    When I turn round, Zdenka is at the printer, facing me. The printer isn't printing.

  - *Otherwise:*

    Nobody saw.

- ▸ **Take a photo of the report for Stojan.**

  *[Headline: Teo leaked an inspection report to his neighbour.]*

  Click. Now Stojan will know what's coming. That's a favour, technically.

  It feels like stealing a stapler from a church.

- ▸ **Leave it where it is.** *[cautious]*

  I put it back exactly where it was, slightly straighter.

  Stojan will find out on Thursday, the normal way.

### Kornel is hovering near your desk.

*Teo's choice, at: kornel · open 08:30 to 11:15 · takes 15 min · `kornel_desk`*

Kornel is forty-one and dresses like a man who's been told what a weekend is but has never been on one.

- *If `kornel_taxi` is 1:*

  **KORNEL:** Teo. I've booked the taxi for 16:40. It's a Škoda. I asked.

  **KORNEL:** The cost breakdown is in your inbox. It includes a tip of ten percent. I hope that's acceptable to you.

- *Or if Kornel remembers: taxi ignored:*

  **KORNEL:** Teo. I sent you a message this morning, about a taxi. It may have gone to the wrong number.

  It didn't go to the wrong number. He's giving me a way out. He does that for people.

- *Otherwise:*

  **KORNEL:** Teo. Are you looking forward to the event?

**KORNEL:** I've prepared something. For tonight. For the team.

**TEO:** Prepared what?

**KORNEL:** It's a surprise. But it rhymes.

Oh no.

- ▸ **Encourage him.** *[kind]*

  **TEO:** Can't wait.

  **KORNEL:** *(overjoyed)* Really?

  He looks so happy that I have to look at the EU poster for a second.

- ▸ **Gently suggest he doesn't.**

  *(A hidden check on Teo's tact.)*

  - *If it goes well:*

    **TEO:** Maybe save it for a smaller crowd. Let it build.

    **KORNEL:** Let it build. Yes. That's very wise.

    He writes *let it build* on a Post-it and sticks it to his own hand.

  - *Otherwise:*

    **KORNEL:** I see.

    **KORNEL:** I'll do it anyway. But thank you for your input.

    So now he's doing it anyway, and it's brave, and that's my fault.

- ▸ **Make an excuse and escape.**

  I tell him I've got a call. I don't have a call. I sit at my desk and hold the phone to my ear like a man listening to the sea.

### Do some actual work.

*Teo's choice, at: desk · at most 2 times · takes 60 min · `work_permits`*

- *If the first time:*

  *→ minaret*

- *Otherwise:*

  *→ bench*

#### minaret

Application 2231. A man in Lower Varosh wants to build a decorative tower on his garage. Eleven metres. "In the style of a minaret but not religious, for the view."

Attached is a drawing of the tower, with the man at the top, waving.

- ▸ **Refuse it. Height limit is seven metres.**

  Refused, under section 4.2. For a second I feel like a god. A small, boring god of section 4.2.

- ▸ **Ask for more information.**

  I write back asking what he means by "for the view." I actually want to know. I think about him for the rest of the morning.

- ▸ **Approve it. Why not.** *[bold]*

  *[Headline: Teo approved an eleven-metre tower on a garage.]*

  Approved. Somewhere in Lower Varosh a man is going to wave at the whole city.

  I don't know why I did that. It felt like freedom. It'll come back as paperwork.

#### bench

Application 2236. The *Innovation Bench*. EU-funded. A bench with a solar panel and a USB socket, for the river promenade.

Budget: four hundred thousand euro.

There's a consultant's report, ninety pages long. Page sixty-one says the bench will "activate public space."

- ▸ **Approve it. It's not your money.**

  As far as this building is concerned, EU money falls from the sky. Some of it lands on benches.

- ▸ **Flag the budget to Viktor.** *[cautious]*

  I write Viktor a short, careful email. I delete "frankly." I send it.

  He replies in under a minute: *Good catch buddy. Leave it with me.*

  I've never seen anything come back from Viktor's "leave it with me."

### Get some lunch.

*Teo's choice, at: door · open 12:00 to 14:00 · takes 45 min · `lunch`*

Lunchtime.

- ▸ **The canteen, with Kornel.** *[kind]*

  The canteen has two options: soup, and the other soup. Kornel has both.

  **KORNEL:** Did you know the river is a hundred and four kilometres long? Most people think it's a hundred.

  **KORNEL:** I don't know why I told you that.

  I sort of like him. I'd never tell him. He'd write it down.

- ▸ **A beer at the kiosk by the tram stop.** *[drink]*

  A beer at the kiosk, standing up, next to two men from the gas board who nod at me like we're in a club. We are in a club.

  One beer, at lunch, on a Tuesday. I count it as half.

- ▸ **Skip it. Sit at your desk.**

  I eat a cereal bar from the drawer. It's been there since winter. It tastes like winter.

### Look busy.

*Teo's choice, at: desk · can be done again · takes 30 min · `look_busy`*

- *One of these, at random, each only once:*

  - I open a spreadsheet and look at it. Half an hour goes by. I couldn't tell you what was in it.

  - I answer an email from March. It says "Any update?" I write "Following up." That's most of the job.

  - I wash a mug in the kitchen. It isn't mine. It takes four minutes and it's the best part of the morning.

  - Down in the street a man spends twenty minutes trying to park a van. I watch all of it. When he gets it in, I nearly clap.

  - I print a form, so I can carry it to the printer and back. Walking with paper looks like work.

  - Kornel asks if I've seen the stapler. I haven't. We look for it together for a while. It's on his desk.

  - I read the minutes of a meeting I was in. I don't remember it. According to the minutes, I agreed.

  - Zdenka walks past my desk. I scroll down, with purpose.

- *If done 8 or more times before:*

  I've run out of ways to look busy. I watch the screen until it goes dark, then I move the mouse.

### Keep your head down until it's time to go.

*Teo's choice, at: desk · open 13:30 to 15:45 · `head_down`*

*(Note: The dead stretch of the afternoon, skipped in one go.)*

I keep my head down. Emails. A form about another form. At some point the sun moves off my desk and onto Kornel's.

Then it's quarter to four, and people are starting to look at their coats.

### Leave for the party.

*A way out · open 15:45 to 16:30 · takes 30 min · `office_leave`*

- *If `kornel_taxi` is 1:*

  The taxi is a Škoda. Kornel sits in the front so he can talk to the driver about routes.

  The driver has never been happier to arrive anywhere.

- *Otherwise:*

  I walk, along the river, past the spot where the Innovation Bench will go, if it ever does.

  The air is warm and smells of linden and diesel. It's a beautiful evening. I don't trust it.

### Office timeout

*When time runs out · takes 30 min · `office_timeout`*

At half past four, Magda starts herding everyone towards the lifts.

**MAGDA:** Come on, come on! The city's paying!

- *If `kornel_taxi` is 1:*

  Kornel's taxi is downstairs. It's been waiting since 16:25. He wanted a margin.

## TUESDAY 17:00. TERASA VELKA, ON THE RIVER

### Party arrive

*On arrival · `party_arrive`*

Terasa Velka sits on the river like it's waiting for a yacht. By the door there's a brass plaque: *Co-financed by the European Union*.

Everything nice in this city has that plaque. If they ever put one on a person, I'll know they've made it.

*[Magda is now at/doing: door]*

*[Viktor is now at/doing: railing]*

*[Zdenka is now at/doing: table]*

Magda is at the entrance with an envelope of drink tickets, handing them out two at a time, like communion.

**MAGDA:** Two each! Viktor says! I'm the bank!

- *If `magda_paid`:*

  She gives me two, and then, when nobody's looking, a third.

The river goes past, slow and brown.

- *If Teo is open (warmth 2 or more):*

  Everyone's here. I like them more than I'd ever say.

- *Or if warmth 0 or less:*

  Everyone's here. I've already found the exits.

- *If `outsider_consultants`:*

  At the next table, a group in linen shirts and lanyards is having what a laminated sign calls a *Team Alignment Evening*. They're foreigners. You can tell because they're photographing the bread.

- *If `outsider_hen`:*

  At the far end of the terrace, a hen party has taken three tables and a speaker. The bride is wearing a veil and a sash that says *LAST NIGHT OF FREEDOM*. It's Tuesday. The wedding's on Saturday.

- *If `outsider_developer`:*

  A man in a very good suit is alone at the bar, drinking something brown and expensive, watching our party like it's a property for sale.

### Ramona arrives

*17:50, happens by itself · `ramona_arrives`*

A black car pulls up at the kerb. The kind of black that costs extra.

Ramona Baranyi gets out like the pavement owes her money.

*[Ramona is now at/doing: bar]*

She's Viktor's age. White dress, big sunglasses at a quarter to six, and a handbag worth about a year of my salary.

*(Note: How she greets him sets Viktor's evening, and whether he opens the bar (viktor_rounds).)*

- *If a 60% chance:*

  Viktor goes to kiss her cheek. She turns her head a perfect amount, so he gets air.

  **RAMONA:** Darling. You didn't say it was outdoors.

  **VIKTOR:** It's summer, Ramona.

  **RAMONA:** I'm aware of the season.

- *Otherwise:*

  Viktor goes to kiss her cheek. She lets him. Both sides, slowly, like it's a favour he'll hear about later.

  **RAMONA:** Darling. Outdoors. How brave of you.

  **VIKTOR:** For you, Ramona, I'd hold it in a car park.

  She doesn't laugh. But she takes the arm he offers, and he stands up straighter, like a man who's won something.

At the ticket table, Magda has stopped smiling. Nobody else would notice. I notice because I'm looking at her. I don't know where else to look.

### Viktor rounds

*18:35, offstage (Teo may only hear it) · `viktor_rounds`*

- *If (Viktor's own evening is less than 0 (-5 bad to 5 good) and a 85% chance) or (Viktor's own evening is 0 or more (-5 bad to 5 good) and a 20% chance):*

- *Otherwise:*

  - *If `ramona_snubbed`:*

    **VIKTOR:** *(at the bar)* Laci. Something cold. Just for me.

  - *Otherwise:*

    **VIKTOR:** *(at the bar)* Laci! The good bottle. Two glasses. Two.

### Tickets collapse

*18:40, happens by itself · only if `viktor_buys` · `tickets_collapse`*

*(Note: Not every evening: only when Viktor needs to look big (viktor_rounds).)*

At twenty to seven, the ticket system collapses.

Viktor does it. He slaps the bar, says something to Laci, and suddenly there are trays of little glasses. Zhivka: plum spirit that tastes like a grandmother's revenge.

**VIKTOR:** One for the team! One! It's a Tuesday!

It's never one.

*[Headline: The two-ticket rule lasted one hour and forty minutes.]*

- ▸ **Take one.** *[drink]*

  It goes down like a lit match. Everybody cheers, so I cheer. I don't know what for. Viktor, probably.

- ▸ **Pretend to take one. Pour it in a plant.** *[cautious]*

  I tip it into a potted olive tree. Zdenka, three metres away, watches me do it.

  She raises her mineral water, very slightly. That's either respect or a warning.

- ▸ **Take two.** *[drink]*

  **VIKTOR:** Look at this man!

  Second one goes down differently. Second one has opinions.

### Stojan evening

*19:00, offstage (Teo may only hear it) · `stojan_evening`*

- *If `stojan_refused`:*

  *[Stojan is now at/doing: building]*

- *Or if `stojan_promise`:*

  *[Stojan is now at/doing: celebrating]*

### Kornel poem

*20:00, happens by itself · only if not `kornel_talked_down` · `kornel_poem`*

Kornel taps a glass with a fork. Then taps it again, because the first time nobody heard. Then a third time, which hits a note, and somehow that's worse.

**KORNEL:** Colleagues. I have written something. For the team.

He unfolds a piece of A4. Typed. Double-spaced. It has a title.

**KORNEL:** "Ode to Permits."

Somebody whoops. It isn't a kind whoop.

**KORNEL:** "In a building made of concrete, in a city by the Vel..."

- ▸ **Go up and stand beside him. In solidarity.** *[kind] (only if not (Teo is careful (nerve -2 or less) and Teo is closed (warmth -1 or less)))*

  *(A hidden check on Teo's nerve.)*

  I walk up and stand next to him. I don't know why. My legs did it. Kornel looks at me like I've pulled him out of a well.

  - *If it goes badly:*

    It doesn't help. Now there are two of us up there.

    Someone films it.

  - *Otherwise:*

    I start clapping on the rhymes. Then Magda does, then the next table, ironically at first and then not, because it's actually quite catchy, and by "...and Zdenka, who has measured all of us, and found us well," the whole terrace is cheering.

    Zdenka, for one second, looks almost moved.

    *[Headline: Kornel's poem brought the house down. Teo stood next to him.]*

  *[a hindsight line, see Shared lines]*

- ▸ **Film it.** *[bold]*

  *[Headline: Teo filmed Kornel's poem. The video exists.]*

  I film it. Everybody's filming it.

  Kornel sees the phones. All of them. He reads to the end anyway. That's the bravest thing I've seen in years and I recorded it for laughs.

- ▸ **Look away.**

  *(Left to luck.)*

  - *If it goes well:*

    I look at the river. Behind me, against all odds, people start clapping along. By the end, they're cheering. Kornel has never been cheered in his life. He bows too low and nearly falls.

    *[Headline: Kornel's poem was, somehow, a hit.]*

  - *Otherwise:*

    I look at the river. It takes four minutes.

    When I turn back, Kornel's folding the A4 very small, and Viktor is clapping, slowly, with a big smile.

    *[Headline: Kornel read his poem. It went badly.]*

### Magda breaks

*20:15, happens by itself · only if not `viktor_foiled` · `magda_breaks`*

At about quarter past eight, something happens. Nobody announces it.

Magda finally goes over to Viktor, in the red dress, with fresh lipstick and a drink for him in her hand.

He's with [Felicity / the maid of honour / the new waitress]. He takes the drink. He says something to Magda with his warm, generous, professional smile, the one he uses for pensioners at public consultations.

Then he turns back to [Felicity / the maid of honour / the new waitress], the way you close a file.

Magda stands there with nothing in her hands.

Then she goes down the steps to the river walk, quickly, not running, because running would mean something.

*[Magda is now at/doing: river]*

### Kornel with zdenka

*20:45, offstage (Teo may only hear it) · only if `kornel_poem` has happened and `kornel_poem` is 0 and not `stood_with_kornel` and not Zdenka is at/doing: gone · `kornel_with_zdenka`*

*[Kornel is now at/doing: with zdenka]*

**ZDENKA:** *(at Zdenka's table)* It rhymed, Kornel. That's more than most of them manage.

### Kornel first beer

*21:00, offstage (Teo may only hear it) · only if `kornel_poem` is 2 · `kornel_first_beer`*

- *If `outsider_hen`:*

  **DENISA:** *(from the hen party)* A beer for the poet!

**KORNEL:** It tastes like bread that's given up!

### Viktor to club

*21:30, offstage (Teo may only hear it) · only if Viktor is at/doing: railing and not `viktor_foiled` and (not `viktor_invite` or it's 22:30 or later) · `viktor_to_club`*

*[Viktor is now at/doing: club]*

**VIKTOR:** *(by the railing)* Schengen! Who's young? Nobody? Fine. Come anyway.

### Magda finds teo

*22:00, happens by itself · only if (Magda is at/doing: bar or Magda is at/doing: hen) and Magda's regard for Teo is 2 or more (-10 to 10) and not `magda_walk_offer` and not `came_to_teo` · `magda_finds_teo`*

Magda finds me. Shoes in one hand, bag in the other, like a woman leaving a burning building in no particular hurry.

**MAGDA:** Teo. Walk me home? I don't want the tram looking like this.

- ▸ **Walk her home.** *[kind]*

  **TEO:** Now?

  **MAGDA:** Now. Before I say goodbye to everyone. That takes an hour, with me.

  *[Magda is now at/doing: gone]*

  *[Goes to: Outside Magda's building]*

- ▸ **Not tonight, Magda.**

  **MAGDA:** No. Course. You're right. I'm a grown woman. I've got a phone. I've got an app.

  She hasn't got the app.

### Magda leaves with someone

*22:00, offstage (Teo may only hear it) · only if (Magda is at/doing: bar or Magda is at/doing: hen) and not `magda_walk_offer` and (Magda's regard for Teo is less than 2 (-10 to 10) or it's 22:30 or later) · 40% chance · `magda_leaves_with_someone`*

*(Note: Nobody is taking her home, so she goes with someone. Who it is, is luck.)*

*[Magda is now at/doing: gone]*

- *If `outsider_consultants` and a 50% chance:*

  *[Headline: Magda left the party with one of the consultants.]*

  **MAARTEN:** *(by the way out)* I'll get you a taxi. I'll come with you. To make sure you get home.

- *Or if `outsider_developer` and a 50% chance:*

  *[Headline: Magda left the party in Adrian Kovál's car.]*

  **ADRIAN:** *(by the way out)* My driver's outside. He goes wherever you like.

- *Otherwise:*

  *[Headline: Magda left the party with a man nobody knew.]*

  **MAN:** *(by the way out)* I know a place. It's open late. Everything's open late if you know a place.

### Viktor decides

*22:00, offstage (Teo may only hear it) · only if Viktor is at/doing: bar and not `viktor_decided` · `viktor_decides`*

*(Note: Ten o'clock, if Teo ruined his evening: half the time he comes for Teo, half the time he drinks on alone.)*

- *If not `came_to_teo` and a 50% chance:*

### Zdenka leaves

*22:00, offstage (Teo may only hear it) · only if not Zdenka is at/doing: gone · `zdenka_leaves`*

*[Zdenka is now at/doing: gone]*

- *If `zdenka_dark` and not `saw_zdenka_napkin`:*

### Ramona leaves

*22:30, offstage (Teo may only hear it) · only if Ramona is at/doing: bar and not `ramona_offer_ride` and (not `ramona_will_ask` or `came_to_teo`) · `ramona_leaves`*

*(Note: Half ten: she goes. If her evening has gone badly, sometimes she takes Teo with her.)*

- *If Ramona's own evening is -2 or less (-5 bad to 5 good) and not `came_to_teo` and a 50% chance:*

- *Otherwise:*

  *[Ramona is now at/doing: gone]*

  **RAMONA:** *(at the bar)* Bogdan. We're going. No, I'm not saying goodbye. They'll work it out.

### Viktor leaves alone

*22:45, offstage (Teo may only hear it) · only if Viktor is at/doing: bar and `viktor_decided` and (not `viktor_will_ask` or `came_to_teo`) · `viktor_leaves_alone`*

*[Viktor is now at/doing: club]*

**VIKTOR:** *(at the bar)* Laci. Call me a car. No. Call me two. I'll take the bigger one.

### Party too far

*Happens by itself · only if Teo is wrecked or worse and not `fell_in_river` · `party_too_far`*

The terrace tilts. It's not the terrace.

Laci is saying something. Laci's got two faces, and both of them are worried.

- *If Kornel's regard for Teo is 2 or more (-10 to 10):*

  *→ kornel catch*

- *Otherwise:*

  *→ drift*

#### kornel catch

- *If `kornel_first_beer`:*

  And then Kornel is there. Kornel, who had one beer, two hours ago, and has done the maths.

- *Otherwise:*

  And then Kornel is there. Kornel, who doesn't drink, who's had three Fantas and a philosophy about each.

**KORNEL:** I've got you. I've got the car. Come on. Mind the step. There's a step. That was the step.

*[Headline: Kornel took Teo home. To Kornel's home.]*

*[Goes to: Kornel's car]*

#### drift

I walk away from it all, down to the river, to feel the air, to make the terrace stop doing that.

The river's very close now. Very friendly.

*[Headline: Teo went into the river.]*

*[Goes to: The river]*

### Magda drinks

*Offstage (Teo may only hear it) · only if `open_bar` and Magda is at/doing: door · `magda_drinks`*

**MAGDA:** *(by the door)* Laci! I'm the bank! The bank says yes!

### Magda comes back

*Offstage (Teo may only hear it) · only if Magda is at/doing: river and not `magda_walk_offer` and it's 60 minutes after `magda_went_down` or later · `magda_comes_back`*

*(Note: An hour after Viktor turned away, if nobody came down for her, she comes back up.)*

- *If `outsider_hen`:*

  *[Magda is now at/doing: hen]*

  **DENISA:** *(from the hen party)* Girls! She's one of us now!

- *Otherwise:*

  *[Magda is now at/doing: bar]*

  **MAGDA:** *(at the bar)* Laci. The bank is open again.

### Viktor takes teo

*Happens by itself · only if `viktor_will_ask` and Viktor is at/doing: bar and not `came_to_teo` · `viktor_takes_teo`*

Viktor has been at the bar for an hour, drinking alone. I've never seen him do anything alone. He has people for it.

He puts a hand on the back of my neck. Not hard. Just so I know it's there.

**VIKTOR:** *(displeased)* You. You're coming. You owe me an evening.

- ▸ **Go with him.**

  *[Viktor is now at/doing: club]*

  I go with him. I ruined his evening. The least I can do is watch him fix it.

  *[Goes to: Club Schengen]*

- ▸ **Not tonight, Viktor.**

  **VIKTOR:** No. Course not. You've had a big night. You've been very busy.

  He goes on his own. The car door shuts like a full stop.

  *[Viktor is now at/doing: club]*

### Ramona sees viktor go

*Offstage (Teo may only hear it) · only if Viktor is at/doing: club and Ramona is at/doing: bar and not `ramona_saw_viktor_go` · `ramona_sees_viktor_go`*

**RAMONA:** *(at the bar)* The bottle, please. Not a glass. I'm not a child.

### Ramona finds teo

*Happens by itself · only if `ramona_will_ask` and Ramona is at/doing: bar and not `came_to_teo` · `ramona_finds_teo`*

*[Ramona is now at/doing: gone]*

Ramona stops next to me on her way out. Sunglasses on again, at night. The bag on her arm like a small dog that bites.

**RAMONA:** You. Permits. I'm leaving. You look like a man who needs a lift.

- ▸ **Take the lift.**

  I follow her to the car. Viktor isn't here to see it. I can't tell if that's a relief or a waste.

  *[Goes to: The back of Ramona's car]*

- ▸ **I'm fine, thanks.**

  **RAMONA:** Nobody's fine, darling. Some of us just have a driver.

### Get a drink.

*Teo's choice, at: bar · can be done again · takes 15 min · `party_drink`*

- *If the first time:*

  The barman is about twenty-three. His badge says LACI and under that, smaller, TRAINEE. He's done office parties before. You can tell by how he stands.

- *If not `open_bar` and Teo has 0 or less drink tickets left:*

  *→ no tickets*

*(Note: What the bar is like depends on how drunk Teo already is.)*

- *If Teo is very drunk or worse:*

  - *One of these, at random:*

    - The bar is further away than it was. I get there in the end.

      **LACI:** Water, boss?

      **TEO:** Laci. You're a good man, Laci. Has anyone told you?

      **LACI:** Tonight, four people.

    - I order something. I hear myself order it, very clearly, in a voice I use for funerals.

      Laci waits. Then he puts a beer in front of me, which isn't what I asked for, but it's close.

    - I hold on to the bar with both hands. It's important that the bar stays where it is.

      **LACI:** Same again?

      I nod. The nod takes a while.

- *Or if Teo is drunk or worse:*

  - *One of these, at random:*

    - I lean on the bar the way people lean on bars in films. The bar is wetter than in films.

    - **LACI:** Same again?

      I can't remember what I had. I say yes.

    - The music's better from here. Everything's better from here. I say so to Laci. He agrees, professionally.

- *Or if Teo is tipsy:*

  - *One of these, at random:*

    - Laci is already reaching for a glass. He remembers me. I don't know if that's good.

    - **LACI:** Back again.

      It isn't a question.

- *Otherwise:*

  **LACI:** What can I get you?

- ▸ **A beer.** *[drink] (can be picked again)*

  *[Uses a drink ticket, unless the bar is open.]*

  - *If Teo is drunk or worse:*

    I drink half of it standing at the bar, so I don't have to carry as much.

  - *Otherwise:*

    Cold. Fine. A beer.

- ▸ **Wine. White.** *[drink] (can be picked again)*

  *[Uses a drink ticket, unless the bar is open.]*

  - *If done 1 or less times before:*

    Local white. The vineyard got EU money to stop being sour, and spent it on a sign.

  - *Otherwise:*

    Another white. It's getting less sour as the evening goes on, or I am.

- ▸ **Zhivka.** *[drink] (can be picked again)*

  *[Uses a drink ticket, unless the bar is open.]*

  - *If Teo is drunk or worse:*

    **LACI:** Another one?

    It goes down like water. That's a bad sign. I know it's a bad sign and I have another sip anyway.

  - *Otherwise:*

    **LACI:** Brave.

    It's plum, and then it's fire, and then I feel brave.

- ▸ **Just water.** *[cautious] (can be picked again)*

  - *If Teo is drunk or worse:*

    Water. Laci looks relieved. He's been worried about me, I realise. Laci, who I met three hours ago.

  - *Otherwise:*

    Water. Laci looks at me with something like respect. Or pity. On Laci they're the same face.

#### no tickets

*(Note: Both tickets gone, before Viktor opens the bar.)*

**LACI:** Ticket?

I haven't got one. Two tickets, and I've used them both. It's like a ration book from the war.

- ▸ **Pay for a beer yourself.** *[drink] (only if Teo has 8 euro or more) (can be picked again)*

  *[Money -8 euro]*

  Eight euro for a beer. I pay. Laci doesn't watch me count it out, which is kind of him.

- ▸ **Pay for a beer yourself.** *[drink] (only if Teo has less than 8 euro) (can be picked again)* *→ broke*

- ▸ **Just water.** *[cautious] (can be picked again)*

  Water is free. Laci gives it to me in a wine glass, so nobody can tell.

- ▸ **Leave it.** *(can be picked again)*

  I leave it. I stay at the bar anyway, so I've got somewhere to stand.

#### broke

*(Note: The money's gone (the gift took most of it). Being broke changes the evening; it doesn't end it.)*

- *If not `broke`:*

  *[Headline: Teo ran out of money at his own office party.]*

I put what's in my wallet on the bar. Coins, mostly. It isn't eight euro. It isn't near.

Laci looks at the coins and then somewhere else, to be polite.

*(Note: He can borrow from each of them once; water is always there.)*

- ▸ **Ask Kornel to lend you ten.** *(only if Kornel's regard for Teo is 0 or more (-10 to 10) and not `owes_kornel`) (can be picked again)*

  *[Money +2 euro]*

  Kornel is two metres away, guarding his Fanta. He gets his wallet out before I've finished asking.

  **KORNEL:** Of course. Is ten enough? Take ten.

  He types something into his phone afterwards. He doesn't hide it.

- ▸ **Ask Laci to put it on a tab.** *(only if not `owes_laci`) (can be picked again)*

  **LACI:** There's no tab. It's a private party.

  He looks at the door to the kitchen. Then he pours it anyway and slides a napkin across with a pen.

  **LACI:** Name and number. You pay me Friday.

  I write it down. My handwriting looks like someone else's.

- ▸ **Ask for water, as if that's what you came for.** *[cautious] (can be picked again)*

  **LACI:** Water.

  He gives it to me in a wine glass, so nobody can tell. He's done this before, for someone.

### Magda, in the red dress.

*Teo's choice, at: magda · open 17:00 to 20:30 · takes 25 min · `magda_early`*

She's wearing it, the red dress. She looks good. She's trying very hard, and it's working.

- *If Magda remembers: kind dress:*

  **MAGDA:** *(happy)* You were right! Look! He looked at me! When I came in. He looked.

- *Or if Magda remembers: honest dress:*

  **MAGDA:** I wore it anyway. Like I said.

  **MAGDA:** He hasn't noticed. That's fine. It's early.

- *Or if Magda remembers: cruel dress:*

  She sees me coming and turns a little towards the river. She's still angry, and she's right to be.

- *Otherwise:*

  **MAGDA:** Do I look ridiculous? Don't answer. I look ridiculous. Have a drink with me.

- *If `viktor_wingman` is 1:*

  Over her shoulder, Viktor gives me a little nod. Keep her busy. Good boy.

- ▸ **Have a drink with her.** *[drink]*

  We drink. She shows me photos of her kids. A boy of five, a girl of two, two different fathers. Both kids live with their dads. She says it fast, the way you pull off a plaster, and then she orders another one.

  **MAGDA:** You're nice to talk to, Teo. You don't do anything with your face.

  I think that's a compliment.

- ▸ **Ask her to dance.** *[bold]*

  *(A hidden check on Teo's nerve.)*

  - *If it goes badly:*

    I dance like a man trying to get a wasp out of his shirt. Magda is very kind about it.

    Somebody films it. There's always somebody.

  - *Otherwise:*

    We dance. Badly, but together, and she laughs with her whole face for the first time today.

    For four minutes, nobody's looking at Viktor. Not even her.

  *[a hindsight line, see Shared lines]*

- ▸ **Steer her away from Viktor.** *[cautious]*

  I keep moving us, slowly, towards the far end of the terrace.

  Either she doesn't notice, or she notices and lets me.

  - *If `viktor_wingman` is 1:*

### Viktor is holding court by the railing.

*Teo's choice, at: viktor · open 18:00 to 21:30 · only if Viktor is at/doing: railing · takes 25 min · `viktor_railing`*

Viktor has a cigar he doesn't light. It's a prop. It works.

He's talking to [Felicity / the maid of honour / the new waitress], and [Felicity / the maid of honour / the new waitress] is laughing in the right places. To Viktor, that's a conversation.

Ramona is at the bar with her back to all of it, which means she can see all of it in the mirror.

- *If `viktor_wingman` is 1:*

  **VIKTOR:** Teo! My man. Come here, come here. This is Teo, he's the brains of the department.

  And then, quietly, while [Felicity / the maid of honour / the new waitress] looks at the river:

  **VIKTOR:** Where's Magda?

- ▸ **Do your job. Laugh at his stories.** *(only if `viktor_wingman` is 1)*

  I laugh at the story about the minister, and the one about the boat. For the one about his first car I make a face I didn't know I had.

  **VIKTOR:** Club Schengen later. You're coming. No arguments.

  He's chosen me. I hate how good it feels.

- ▸ **Stay and watch.**

  I stay and watch. Viktor's hand finds the small of her back. It knows the way.

- ▸ **Ruin it. Tell [Felicity / the maid of honour / the new waitress] about Ramona.** *[bold] (only if Teo is drunk or worse or Teo is bold (nerve 2 or more))*

  *(A hidden check on Teo's nerve.)*

  **TEO:** Have you met his wife? She's lovely. She's just there.

  - *If it goes badly:*

    *[Headline: Teo told Viktor's date about Viktor's wife. Viktor saw.]*

    [Felicity / the maid of honour / the new waitress] just laughs. She knew. Everybody knew.

    Viktor doesn't laugh. Viktor looks at me with complete calm, the way you look at a spreadsheet with an error in it.

  - *Otherwise:*

    *[Viktor is now at/doing: bar]*

    *[Headline: Teo ruined Viktor's evening. On purpose.]*

    [Felicity / the maid of honour / the new waitress] looks over at the bar, at the white dress and the handbag, and does the sum.

    She excuses herself. Viktor watches her go. Then he turns to me, smiling, and the smile is the frightening part.

    **VIKTOR:** *(displeased)* Good party, Teo?

  *[a hindsight line, see Shared lines]*

- ▸ **Leave him to it.**

  I go. None of my business. I say that about most things.

### Go down to the river walk, where Magda went.

*Teo's choice, at: steps · only if Magda is at/doing: river · takes 30 min · `magda_river`*

She's on an old bench with a plaque for someone who died in 1991 and "loved this view."

Her mascara has gone. Not run. Just gone.

**MAGDA:** *(sad)* Don't. I know. Don't.

**MAGDA:** Thirty-four tickets. I had a plan. I don't know what the plan was. Get drunk with him. And then.

**MAGDA:** And then what, Magda? Then nothing. Then Wednesday.

She laughs, and it's terrible, and a bit funny, and she knows it. Magda always gets the joke, even when it's on her.

- ▸ **Sit with her. Say nothing.** *[kind] (only if warmth 0 or more)*

  I sit. The river goes past. A bottle goes past on the river. We both watch the bottle.

  **MAGDA:** It's going to the sea.

  **TEO:** It's going to a dam in Hungary.

  She laughs, properly this time, and leans on my shoulder. She smells of perfume and zhivka.

  **MAGDA:** Walk me home later? I don't want to get the tram looking like this.

- ▸ **Tell her the truth about Viktor.** *(only if honesty -1 or more)*

  *(A hidden check on Teo's tact.)*

  **TEO:** He's not worth it, Magda. He's not even... He does this. He does this with everyone.

  - *If it goes badly:*

    **MAGDA:** Everyone. Right. Everyone but me, then.

    I've made it worse. I've made it into maths.

  - *Otherwise:*

    **MAGDA:** I know. I know he does.

    **MAGDA:** But he did it to everyone else, and I wanted a turn. Isn't that stupid?

    **TEO:** It's not stupid.

    It is stupid. But it's the least stupid thing anyone's said tonight.

    **MAGDA:** Walk me home later?

  *[a hindsight line, see Shared lines]*

- ▸ **Kiss her.** *[bold] (only if Teo is drunk or worse or Teo is hungry (appetite 1 or more))*

  *(A hidden check on Teo's nerve.)*

  - *If it goes badly:*

    She turns her face. Kindly. So kindly that I want to walk into the river.

    **MAGDA:** Oh, Teo. No. You're sweet. You're drunk and sweet.

  - *Otherwise:*

    *[Headline: Teo kissed Magda by the river.]*

    She kisses me back. It tastes of zhivka and lipstick, and of both of us being somebody's second choice. We keep going anyway.

    **MAGDA:** Walk me home?

  *[a hindsight line, see Shared lines]*

- ▸ **Go back up to the party.**

  **TEO:** I'll get you a water.

  I don't get her a water. I go back up the steps, and it's like coming up from underwater, back into the music and the lights, and I don't go back down.

  - *If `viktor_wingman` is 1:*

    I was meant to keep her happy. That was my one job.

### Ramona is alone at the bar.

*Teo's choice, at: ramona · open 19:00 to 25:00 · only if `ramona_arrives` has happened · takes 25 min · `ramona_bar`*

Ramona is on her third glass of champagne. She holds it by the stem.

Up close she's fifty, and has had good, expensive work done. Her eyes are tired anyway.

**RAMONA:** You're one of his.

**TEO:** I work in Permits.

**RAMONA:** That's what I said.

She watches Viktor in the mirror behind the bar. She's been watching him in mirrors for thirty years.

**RAMONA:** Do you know what this bag cost?

**TEO:** No.

**RAMONA:** Four thousand euro. I bought it in March. In March he went to a conference in Vienna for four days.

**RAMONA:** There's a bag for every conference. I have a whole wardrobe of conferences.

- *If `ramona_saw_viktor_go`:*

  She has a bottle now, not a glass.

  **RAMONA:** He's gone dancing. He doesn't dance. He goes to watch other people dance and pick one.

- ▸ **Listen.** *[kind] (only if warmth 0 or more)*

  I listen. She talks about Vienna, and about a wardrobe, and about a girl from Viktor's gym in 2009. She talks like a woman reading out the minutes of a long meeting.

  **RAMONA:** You're easy to talk to. Nobody talks to me at these things. They're frightened I'll tell Viktor something.

  **RAMONA:** I never tell Viktor anything. That's the arrangement.

- ▸ **Flirt with her.** *[bold] (only if Teo is drunk or worse or Teo is hungry (appetite 1 or more))*

  *(A hidden check on Teo's tact.)*

  - *If it goes badly:*

    **RAMONA:** Oh, darling. No.

    She pats my hand.

  - *Otherwise:*

    **RAMONA:** Careful. I'm the boss's wife.

    But she's smiling, and this time her eyes are in it.

  *[a hindsight line, see Shared lines]*

- ▸ **Ask her why she stays.** *[bold]*

  *(A hidden check on Teo's tact.)*

  - *If it goes badly:*

    **RAMONA:** Why do you stay in Permits?

    I don't have an answer. She doesn't wait for one.

  - *Otherwise:*

    She actually thinks about it.

    **RAMONA:** Because I was nineteen when I met him, and he was the most alive thing in this city. And he still is. That's the problem.

    **RAMONA:** And because I like the bags.

    She laughs. It sounds like it's been in storage.

- ▸ **Excuse yourself.**

  **RAMONA:** Of course. Run along.

  I run along.

- *If (`ramona_confided` or `ramona_flirted`) and it's 21:00 or later:*

  **RAMONA:** I'm leaving soon. Bogdan's driving. Do you want a lift? I'm not going to stay and watch the rest of this.

### Zdenka is sitting alone, with mineral water.

*Teo's choice, at: zdenka · only if not Zdenka is at/doing: gone · takes 20 min · `zdenka_table`*

Zdenka has a table to herself. She's chosen it so the river's behind her and the whole party is in front, like a stage.

**ZDENKA:** Sit, if you're going to.

- *If `file_seen`:*

  **ZDENKA:** The trolley is very interesting today, I hear.

  She says nothing else. I look at the tablecloth.

- *If Zdenka remembers: asked to bury:*

  **ZDENKA:** Your neighbour. Did you tell him I'm not for sale?

We watch. Kornel is explaining something to a potted olive tree. Behind him, Magda laughs too loud.

**ZDENKA:** I come every year. People drink for free, and then they tell me things.

- ▸ **Ask her what she's found out.**

  **ZDENKA:** Viktor buys rounds so nobody notices he's the reason for them. Kornel wants a friend so badly you can smell it.

  **ZDENKA:** And you watch. Watching isn't the same as being good, Teo.

- ▸ **Ask why she even came.**

  **ZDENKA:** Thirty-one years in that office. I've been to thirty-one of these.

  **ZDENKA:** I like to see everyone together. In one place. It's tidy.

  - *If `zdenka_dark`:*

    She folds her napkin in half. Then in half again. Then again, until it's a hard little square, which she puts in her handbag, like she'll need it later.

- ▸ **Make your excuses.**

  **ZDENKA:** Mm.

### Join the foreigners at the Team Alignment Evening.

*Teo's choice, at: consultants · open 17:30 to 22:30 · only if `outsider_consultants` · takes 25 min · `consultants_table`*

They're from Brussels Advisory Partners. They're here for three weeks to "co-create a roadmap" for the river promenade. They've been here nine days and already have a favourite local word, which is wrong.

**MAARTEN:** You're from here? Really from here?

**TEO:** Really from here.

**FELICITY:** Oh my God, that's amazing. What's it like? Like, actually?

They look at me like I'm a museum with a gift shop.

**MAARTEN:** We think there's so much potential here. The people are so... resilient.

Resilient. That's what they call you when they'd never live where you live.

- ▸ **Give them what they want. Be the authentic local.** *[bold]*

  I tell them about my grandmother, who fought a bear. I don't have a grandmother who fought a bear. I tell them about the old custom of drinking zhivka from a shoe. There is no such custom.

  **FELICITY:** That's incredible. I'm putting that in the deck.

  It's going to be in the deck. Page sixty-one. *Activating cultural heritage.*

  *[Headline: Teo invented a national custom. It's going in a consultant's deck.]*

- ▸ **Tell them the Innovation Bench costs four hundred thousand euro.** *(only if Teo is blunt (honesty 1 or more) or Teo is tipsy or worse)*

  **MAARTEN:** Yes, but it's not just a bench. It's a platform.

  **FELICITY:** It's a conversation starter.

  **TEO:** It's a bench.

  They look at each other the way doctors look at each other.

  - *If `flagged_bench`:*

    I flagged it to Viktor today. I realise Viktor is the one who invited them.

- ▸ **Let them buy you drinks.** *[drink]*

  They buy a bottle of the most expensive wine on the list. It's the cheap wine with a better label. I don't tell them. That's my gift to Brussels.

### The hen party is waving you over.

*Teo's choice, at: hen · open 18:00 to 23:00 · only if `outsider_hen` · takes 30 min · `hen_party`*

They've decided I'm their mascot. Hen parties pick a man. Tonight it's me.

The bride is called Denisa. She's twenty-six, marrying a man called Robi who sells tyres, and she's having the time of her life, with a deadline.

**DENISA:** You! Office man! Are you married?

**TEO:** No.

**DENISA:** Why not? What's wrong with you? Be honest.

The whole table waits. They really want to know. So do I.

- ▸ **Tell the truth. "Nobody asked."**

  **DENISA:** Oh my God. That's the saddest thing I ever heard. Girls! Nobody asked him!

  They all go *awww*, and one of them gives me her sash. It says *BRIDESMAID*. I wear it for the rest of the night. It's the nicest thing anyone's done for me this year.

- ▸ **Do a shot with the bride.** *[drink]*

  We do a shot. Then another, which was apparently the same shot. Denisa tells me Robi is a good man. She tells me three times. The third time she isn't telling me.

- ▸ **Dance with them.** *[bold]*

  *(A hidden check on Teo's nerve.)*

  - *If it goes badly:*

    I dance. Something happens to my knee. Something else happens to a chair.

    **DENISA:** Office man is down!

    They're delighted. I'm the best thing that's happened to them all night.

  - *Otherwise:*

    I dance with seven women and a speaker, to a song about a tractor, and I'm good. I'm actually good. Nobody from the office sees it.

### The man in the very good suit nods at you.

*Teo's choice, at: developer · open 18:00 to 22:30 · only if `outsider_developer` · takes 20 min · `developer_bar`*

**ADRIAN:** Teo Halász. Permits.

He knows my name. He says it like he's reading it off a list. He's reading it off a list.

**ADRIAN:** Adrian Kovál. Kovál Development. Riverside Residences. You've seen the hoardings.

I've seen the hoardings. A couple drinking wine on a balcony in a building that doesn't exist yet, looking at a view that's mostly our river and partly our sewage plant.

**ADRIAN:** I like your department. Thorough. Mostly. Your boss and I are old friends.

He slides a card across the bar. Thick card. Embossed. Card like that is a message by itself.

**ADRIAN:** If you ever want to talk about your future. Or a flat with a view. We have a scheme for public servants. Very favourable.

- ▸ **Take the card.** *[bold]*

  *[Headline: Teo took a property developer's card.]*

  I take the card. It's heavier than it looks.

  **ADRIAN:** Good man. No hurry. Nothing ever needs to hurry.

- ▸ **Leave it on the bar.** *[cautious]*

  I leave it on the bar. He doesn't take it back. It stays there, between us.

  **ADRIAN:** Of course. Another time.

- ▸ **Ask about the Innovation Bench.**

  **ADRIAN:** Ha! The bench. That's a little thing. A courtesy, for the promenade. Next to my building. You understand.

  I do now.

### Step away to the railing for some air.

*Teo's choice, at: railing · at most 2 times · takes 20 min · `river_air`*

The river smells of summer and mud, and faintly of the sewage plant, which the Riverside Residences brochure calls "the water."

- *If Teo is very drunk or worse:*

  *→ lean*

I stand there and breathe. Across the water the old blocks have their lights on, most of the windows blue from the same programme.

- *If Teo is in a low mood (mood less than 0):*

  I feel slightly better. Not good. Better.

#### lean

I lean on the railing. The railing leans back.

*(A hidden check on Teo's judgment.)*

- *If it goes badly:*

  *→ over*

- *Otherwise:*

  I hold on. The world swings and then settles.

  That was nearly a story people tell.

  *[a hindsight line, see Shared lines]*

#### over

I lean a bit further. To see the lights. The lights are very beautiful from here, upside down.

Then the railing isn't there, and the river is.

*[Headline: Teo went into the river.]*

*[Goes to: The river]*

### Magda is back.

*Teo's choice, at: magda · only if Magda is at/doing: bar or Magda is at/doing: hen · takes 20 min · `magda_back`*

- *If Magda is at/doing: hen:*

  The hen party has her now. Someone has put a veil on her, and the bride doesn't seem to mind.

  **MAGDA:** Teo! These are my friends! I met them twenty minutes ago!

- *Otherwise:*

  She's on a stool at the bar with her shoes off and her lipstick back on, slightly to the left of where her mouth is.

  **MAGDA:** Don't ask. I'm fine. I'm having a lovely time. Laci, tell him.

  **LACI:** She's having a lovely time.

- ▸ **Sit with her a while.** *[kind] (only if warmth 0 or more)*

  We don't talk about Viktor. We talk about the tram, and her son's teacher, and whether pigeons have funerals. She thinks yes.

  **MAGDA:** Walk me home later? I don't want the tram looking like this.

- ▸ **Get her a water.**

  This time I actually get the water. She drinks half and uses the rest to clean her face in the mirror behind the bar.

  **MAGDA:** Better?

  It isn't. I say it is. That's what the water was for.

- ▸ **Leave her to it.**

  **MAGDA:** Go on. Go. I'm fine. I said.

### Zdenka's table, empty.

*Teo's choice, at: zdenka table · only if Zdenka is at/doing: gone · takes 5 min · `zdenka_empty_table`*

Zdenka's gone. The chair is pushed in, square to the table. Her glass is gone too. She took her glass.

- *If `zdenka_left_napkin`:*

  There's a napkin on the cloth, folded in half, and in half, and in half again, into a hard little square. Exactly in the middle.

### Go home.

*A way out · open 18:30 to 25:00 · can be done again · takes 30 min · `leave_home`*

*(Note: Before ten, someone may stop him at the gate. Once an evening, and he can always go anyway.)*

- *If it's before 22:00 and not `stopped_at_gate` and a 60% chance:*

  *→ stopped*

*→ going*

#### going

- *If Teo is drunk or worse:*

  I go home the long way, by accident. The city's very beautiful at night if you don't know where you are.

- *Otherwise:*

  I go home. Nobody notices I've gone.

*→ on the way (then back here)*

- *If `magda_river` hasn't happened and Magda is at/doing: river:*

  On the way out, I see Magda's shoes on the steps down to the river. Just the shoes. I keep walking.

#### stopped

*(Note: Who stops him depends on who's there and what's still to come.)*

- *If Magda is at/doing: door:*

  **MAGDA:** You can't go! You've still got a ticket! It's against the rules to go home with a ticket.

  She says it like it's a real rule. Here, it might be.

- *Or if `kornel_poem` hasn't happened and not `kornel_talked_down`:*

  **KORNEL:** You're going? I'm reading at eight. I mean, it's nothing. It's a short thing. It rhymes.

  He's holding a piece of A4, folded in four. He's been holding it all evening.

- *Or if Viktor is at/doing: railing:*

  **VIKTOR:** Teo! Nobody leaves before me. Tradition. I started it tonight.

- *Or if `outsider_hen` and it's before 23:00:*

  **DENISA:** Office man! Where are you going? We haven't finished with you!

- *Otherwise:*

  *→ going*

- ▸ **Stay a bit longer.**

  *[He doesn't leave after all.]*

  - *If `stopper` is 1:*

    **MAGDA:** Good. Now you have to use it. Those are also the rules.

  - *If `stopper` is 2:*

    **KORNEL:** Oh. Good. Not for the poem. Just generally. Good.

  - *If `stopper` is 3:*

    He's already turned back to his company. He just wanted to win.

  - *If `stopper` is 4:*

    They cheer. I don't know what I've agreed to.

- ▸ **Go anyway.**

  - *If `stopper` is 1:*

    **MAGDA:** Fine. Leave me with the bank.

  - *If `stopper` is 2:*

    **KORNEL:** No, of course. It's late. It's not late. But of course.

  - *If `stopper` is 3:*

    **VIKTOR:** Go on, then. Sleep is for the people who work for me.

  - *If `stopper` is 4:*

    **DENISA:** Coward!

    She means it nicely. Mostly.

  *→ going*

### Go on to Club Schengen with Viktor.

*A way out · open 21:00 to 25:00 · only if (`viktor_invite` or Viktor's regard for Teo is 5 or more (-10 to 10)) and not Viktor is at/doing: club · takes 20 min · `leave_club`*

Viktor puts an arm round me like I'm a son he chose.

**VIKTOR:** Come on. The night is young and we're not.

- *If not `viktor_foiled`:*

  [Felicity / the maid of honour / the new waitress] is coming too, in the car behind. Ramona, I notice, is not invited. Ramona has noticed too.

*[Goes to: Club Schengen]*

### Walk Magda home.

*A way out · only if `magda_walk_offer` · takes 10 min · `leave_magda`*

Magda's carrying her shoes. I carry her bag. It weighs about the same as a small child and contains, by the sound of it, mostly keys.

*[Goes to: Outside Magda's building]*

### Take Ramona's lift.

*A way out · open 21:00 to 25:00 · only if `ramona_offer_ride` · takes 10 min · `leave_ramona`*

Ramona doesn't say goodbye to anyone. She walks to the car like she's leaving a hotel that has disappointed her.

Viktor watches her go in the mirror behind the bar. Then he sees me following her. His face doesn't change. That's how I know.

*[Goes to: The back of Ramona's car]*

### Party closing

*When time runs out · `party_closing`*

At one o'clock, the staff start stacking chairs around the last people, like a wall going up.

Laci turns the music off. In the silence, you can hear the river. It's been there the whole time.

- *If Teo is very drunk or worse and Kornel's regard for Teo is 0 or more (-10 to 10):*

  **KORNEL:** Teo. I'm driving. I've had Fanta. Let me drive you.

  *[Goes to: Kornel's car]*

I walk home. Somebody's tied a balloon to a bin. One of Magda's, from the office. It's lasted longer than the party.

*→ on the way (then back here)*

## TUESDAY NIGHT. CLUB SCHENGEN

### Club schengen

*`club_schengen`*

Club Schengen is in a former customs warehouse. The theme is "borderless." The bouncer is the biggest border I've ever seen.

He looks at me. Then at Viktor. Then he unhooks the rope, for Viktor.

Inside it's loud and blue and everyone is twenty years younger than us. Viktor moves through them like it's his living room.

**VIKTOR:** Teo. Teo! This is life. Look at it.

He gets a table. He gets a bottle, with a sparkler in it, carried by a girl who looks embarrassed for all of us.

- *If not `viktor_foiled`:*

  *→ with target*

- *Otherwise:*

  *→ alone with viktor*

#### with target

[Felicity / the maid of honour / the new waitress] arrives twenty minutes later. Viktor stands up for her. He never stands up for anyone at work.

After a while they stop talking. After a while they're in a corner. It's none of my business, and I watch it anyway.

- ▸ **Take a photo.** *[bold]*

  *[Headline: Teo has a photo of Viktor that Viktor doesn't know about.]*

  I don't decide to take it. My hand takes it. The flash is off. My hand thought of that too.

  It's a terrible photo. Blurry, blue, two shapes. Anyone who knows him would know him.

  I don't know what it's for. I put my phone away like it's hot.

- ▸ **Have another drink. Look away.** *[drink]*

  I drink. I look at the dance floor. A boy is dancing with his eyes closed. He looks completely happy. I can't remember the last time I closed my eyes in public.

- ▸ **Leave.**

  I leave without saying goodbye. Viktor doesn't notice. Or he notices and it costs him nothing.

  *→ home*

*→ deal*

#### alone with viktor

Viktor's evening didn't go to plan. He doesn't say whose fault that is.

**VIKTOR:** You know your problem, Teo? You think being honest makes you good.

He pours me a drink. A big one.

**VIKTOR:** It doesn't. It just makes you honest.

- ▸ **Drink it.** *[drink]*

  It burns. He watches me drink it the whole way down, like a test.

- ▸ **Leave it.**

  I leave it on the table. We both look at it.

*→ home*

#### deal

Later, maybe two o'clock, Viktor drops into the seat next to me with his shirt open another button. He looks fresh. He always looks fresh.

**VIKTOR:** I'm restructuring. After summer. Deputy head of section. Somebody who's good, somebody who's loyal.

**VIKTOR:** You've been loyal tonight.

He doesn't say *you saw nothing*. That's part of the job.

- ▸ **Thank him.**

  *[Headline: Viktor half-promised Teo a promotion. In a nightclub. At two in the morning.]*

  **TEO:** Thanks, Viktor.

  **VIKTOR:** Don't thank me. Earn it.

  He means keep earning it.

- ▸ **Say nothing.**

  I say nothing. He takes it as a yes. Everybody always takes my nothing as a yes.

*→ home*

#### home

- *If Teo has 30 euro or more:*

  *[Money -30 euro]*

  The taxi home costs thirty euro. The driver tells me about his brother in Canada the whole way, and I listen like it's the news.

- *Otherwise:*

  *(Note: Broke: no taxi. He walks, and Wednesday starts later and worse.)*

  A taxi is thirty euro. I haven't got thirty euro. I walk.

  It takes an hour and a half. Somewhere along the river the sky goes grey, then pink, behind the sewage plant.

  *→ on the way (then back here)*

## TUESDAY NIGHT. OUTSIDE MAGDA'S BUILDING

### Magda walk

*`magda_walk`*

Magda lives in Block C of the Florian estate. Eleven storeys. The lift works on odd days.

We walk. It's warm. She tells me about the estate when it was new, in 1983, and her mother thought it was the future.

At the door of Block C, she stops. Keys in her hand. All of them.

- *If `kissed_magda`:*

  **MAGDA:** Do you want to come up?

  **MAGDA:** You don't have to. I know what I am tonight. I'm not stupid.

- *Otherwise:*

  **MAGDA:** Thank you. Really. You didn't have to do that.

  **MAGDA:** Do you want a tea? Just a tea. I have a lot of tea. People give me tea.

- ▸ **Go up.** *(only if `kissed_magda`)* *→ up together*

- ▸ **Go up for tea.** *[kind] (only if not `kissed_magda`)* *→ tea*

- ▸ **Say goodnight.** *[cautious]* *→ goodnight*

#### up together

We go up. Eleven floors, because it's an even day.

Her flat is small and very clean. There are two children's drawings on the fridge, and no children.

We're kind to each other, and careful, the way you are with someone you'll see at work in seven hours.

Afterwards she cries a little, and laughs at herself for crying, and I hold her, and I think about Viktor, which is the most disgusting thought I've ever had, and I put it away.

*[Headline: Teo spent the night at Magda's.]*

#### tea

Eleven floors, because it's an even day.

She makes tea. She shows me videos on her phone. Her son's fifth birthday, at his dad's, which she watched on a video call. Her daughter asleep in a car seat in someone else's car.

At some point I'm asleep on her sofa, and at some point she puts a blanket on me that smells of fabric softener and somebody else's life.

*[Headline: Teo fell asleep on Magda's sofa. Just that.]*

#### goodnight

**TEO:** Goodnight, Magda.

**MAGDA:** Goodnight, Teo. You're a nice man. Somebody should tell you.

She goes in. Through the glass I watch her walk past the lift, because it's an even day, and start up eleven floors in her stockings, with her shoes in her hand.

*[Goes to: The stairwell]*

## TUESDAY NIGHT. THE BACK OF RAMONA'S CAR

### Ramona car

*`ramona_car`*

The back of Ramona's car is quieter than my flat. The driver is called Bogdan. In nineteen years he has heard everything and repeated none of it.

Ramona takes her shoes off and puts her feet on the seat like a teenager.

**RAMONA:** Do you know what I wanted to be? At nineteen?

**RAMONA:** An architect. I was going to build things. Then I married a man who decides what gets built. I thought that was the same thing.

She laughs.

**RAMONA:** It's not the same thing.

- *If `ramona_flirted`:*

  *→ close*

- *Otherwise:*

  *→ talk*

#### close

She's looking at me. The streetlights go over her face, one, two, three, like someone flicking through a book.

- ▸ **Kiss her.** *[bold]*

  *(A hidden check on Teo's nerve.)*

  - *If it goes badly:*

    She puts one finger on my lips. Not unkindly.

    **RAMONA:** No, darling. I'm a lot of things. I'm not a revenge.

    Bogdan's eyes in the mirror. Then not.

  - *Otherwise:*

    *[Headline: Teo kissed his boss's wife in the back of her car.]*

    She kisses me back. Whatever this is for her, it started thirty years before me.

    Bogdan turns the radio on. Very quietly. Bogdan is a professional.

- ▸ **Don't.** *[cautious]*

  I look out of the window. She sees me do it.

  **RAMONA:** Sensible. You're very sensible, Teo. It'll keep you exactly where you are.

*→ drop*

#### talk

- ▸ **Tell her she still could. Build things.** *[kind]*

  **RAMONA:** At fifty?

  **TEO:** People build things at fifty.

  **RAMONA:** Men build things at fifty. Women get a bag.

  But she's quiet for a while after that. Not the bad kind of quiet.

- ▸ **Ask about Viktor.**

  **RAMONA:** Viktor? Viktor is Viktor. I knew that when I married him.

  She holds up the handbag, as if that explains the rest.

*→ drop*

#### drop

The car stops outside my building. Ramona looks at it. My building. Panel block, 1979. Someone's spray-painted *NO* on the door, with nothing after it.

**RAMONA:** Goodnight, Teo. Don't become him.

She doesn't say which him. I don't ask.

## TUESDAY NIGHT. THE RIVER

### River night

*`river_night`*

The water is cold. Properly cold, all at once.

- *If Teo is very drunk or worse:*

  It's very hard to tell up from down. I choose one. It's the wrong one.

- *If `outsider_hen`:*

  Then there are hands, a lot of them, with long nails, and a hen party screaming *OFFICE MAN*.

  They pull me out. Denisa holds my head. Someone films it.

  **DENISA:** You're not allowed to die! It's my hen night!

  *[Headline: A hen party pulled Teo out of the river.]*

- *Otherwise:*

  Then there's a hand. Laci. Laci the barman, in the water up to his chest, swearing in three languages.

  **LACI:** Boss. Boss! Why? Why are you like this?

  *[Headline: Laci the barman pulled Teo out of the river.]*

The police arrive, because someone called them, and then an ambulance, because someone else did. The paramedic shines a light in my eye and says I'm fine, just drunk.

The police have a solution for men who are fine and drunk.

**POLICEMAN:** Name?

**TEO:** Teo Halász. Permits.

**POLICEMAN:** Permits. Of course.

The Municipal Sobering-Up Station is on Kollár Street. It has a brass plaque by the door: *Co-financed by the European Union*.

I knew it. I knew they'd put one on a person.

## TUESDAY NIGHT. KORNEL'S CAR

### Kornel night

*`kornel_night`*

Kornel's car is a 2009 Dacia, brown, very clean, with an air freshener shaped like a tree that's never seen a tree.

He drives at exactly the speed limit and stops at every amber light.

**KORNEL:** I don't know your address. I'm going to take you to mine. I hope that's acceptable to you.

- *If Teo is very drunk or worse:*

  I tell him it's acceptable. I tell him he's a great man. I tell him about my father. I don't know which parts are out loud.

Kornel's flat is on the ninth floor of Block F. It's tidy in a way that hurts. Books in order of height. One of everything. One mug, one plate, one chair at the table. A framed certificate: *Kornel Vidrascu, Regional Spelling Bee, 1996, Second Place*.

He makes up the sofa with sheets and hospital corners. He puts a glass of water by my head and a bucket by my feet and a note on the bucket that says *BUCKET*.

- *If `kornel_poem` has happened and `kornel_poem` is 2:*

  **KORNEL:** Tonight was the best night I've had in eleven years.

  He says it to the ceiling. Then he goes to bed.

- *Otherwise:*

  **KORNEL:** Goodnight, Teo. I'm glad you're my colleague.

  He says it to the ceiling. Then he goes to bed.

## TUESDAY NIGHT. THE STAIRWELL

### Home stairwell

*`home_stairwell`*

- *If it's before 22:00 and Teo is tipsy at most:*

  *→ early*

- *If Teo is very drunk or worse:*

  *→ very drunk*

*→ late*

#### early

The stairwell at half past nine. Somebody's frying onions. The daughter's music, faint, through the door: still the disappointed girl.

- *If `stojan_promise`:*

  Stojan opens his door before I reach mine. He's been listening for me.

  **STOJAN:** Mr Permit! Early night! Good man. So? You looked?

  - *If `file_photo`:*

    I show him the photo of the report. He reads it twice. He reads "bus stop" twice.

    **STOJAN:** Fourteen square metres. They measured. Who measures like this?

  - *Otherwise:*

    **TEO:** I'm working on it.

    **STOJAN:** Of course. Of course. Working.

I go in. I sit on the bed in my clothes.

- *If Stojan is at/doing: building:*

  *→ next door building (then back here)*

- *Or if Stojan is at/doing: celebrating:*

  *→ next door party (then back here)*

- *Otherwise:*

  Through the wall, Dana says something, and Stojan laughs, and for a moment they sound like a couple in a film. Then the moment goes.

#### late

Stojan is on the landing again. Same vest. Same cigarette. Possibly he lives here, on the landing.

- *If `stojan_promise`:*

  *→ late promised*

**STOJAN:** Teo. Big night?

**TEO:** Medium night.

**STOJAN:** Medium. Ha. Medium is the worst kind. Nothing to remember.

*→ bed*

#### late promised

**STOJAN:** Mr Permit. So? You fixed?

It's the middle of the night. He's asking me about a toilet.

- ▸ **"Working on it."**

  **STOJAN:** Good man.

- ▸ **"Stojan, it's the middle of the night."**

  **STOJAN:** Problems don't sleep, Teo.

  He's right. Stojan is often right, in a terrible way.

- ▸ **"I'm not going to fix it."** *[bold]*

  He looks at me for a long time. Then he stubs the cigarette out, carefully, on the No Smoking sign.

  **STOJAN:** Okay. Okay, Teo.

  He goes in and closes the door very quietly.

*→ bed*

#### very drunk

Stojan. Stojan on the landing. Stojan is a landing.

**STOJAN:** Mr Permit! Big night!

I love Stojan. Why have I never told him? I need to tell him.

- ▸ **"I'll fix everything, Stojan. Everything."**

  **STOJAN:** This is what I'm saying! Everything!

  He hugs me. He smells of cigarettes and garlic and a kind of hope I'm going to have to deal with.

  *→ bed*

- ▸ **Hug Stojan.**

  I hug Stojan. Stojan allows it. He pats my back twice, hard, the way you burp a baby you're not sure about.

  *→ bed*

- ▸ **Try your key in Stojan's door.** *→ wrong door*

#### wrong door

The key doesn't fit. I try harder. Keys fit if you believe in them.

The door opens from the inside. It's Dana, the wife, in a dressing gown, holding a frying pan like she's done this before.

Behind her, in the hall, the daughter. Phone up. Filming.

**DANA:** Stojan. Your friend.

**STOJAN:** He's not my friend, he's from Permits.

*[Headline: Teo tried to get into the neighbours' flat. The daughter filmed it.]*

*→ bed*

#### bed

- *If Teo is very drunk or worse:*

  I find my door. It's my door. I'm so proud of it.

- *If Stojan is at/doing: building:*

  *→ next door building (then back here)*

- *Or if Stojan is at/doing: celebrating:*

  *→ next door party (then back here)*

I'm asleep before I've finished lying down.

#### next door building

*(Note: Stojan's evening, heard through the wall.)*

Through the wall, a drill. Then a hammer. Then the drill again, as if it lost the argument the first time.

**DANA:** *(through the wall)* Stojan, it's midnight.

**STOJAN:** *(through the wall)* Concrete doesn't know what time it is!

He's building it anyway. Faster, so it's finished before anyone can say no to it again.

#### next door party

Through the wall, music. Accordion, then a pop song, then the accordion again, fighting it. Glasses. Somebody singing in a key that doesn't exist.

**STOJAN:** *(through the wall)* To Teo! To Permits!

**LENKA:** *(through the wall)* Some people need their sleep! For their skin!

He's celebrating. He's celebrating something I said I'd look at.

## WEDNESDAY 07:10. THE MORNING AFTER

### Wake home

*only if Teo slept at home or Teo slept at home · `wake_home`*

- *If Teo's hangover is 3 or more (0 to 3):*

  I wake up because my mouth has become a carpet. Somebody else's carpet. From a bar.

  My shoes are on. One of them is on the wrong foot, which shouldn't be possible, and yet.

- *Or if Teo's hangover is 2 or more (0 to 3):*

  I wake up at ten past seven with a headache shaped like Viktor.

- *Otherwise:*

  I wake up at ten past seven, rested, which feels like cheating.

- *If Teo is hungover:*

  Everything is too bright and slightly my fault.

- *If `walked_home`:*

  My feet hurt. I walked home from Club Schengen because I couldn't pay for a taxi. I remember the sunrise and not much else.

- *If `wearing_sash`:*

  I'm still wearing the sash. *BRIDESMAID*. I leave it on for a while. Nobody can see me.

- *If `kissed_ramona`:*

  I lie there and remember the car. The leather. The radio, very quietly.

  I kissed my boss's wife. On a Tuesday. With the driver watching.

  I put the pillow over my face. It doesn't help.

*→ phone check*

### Wake magdas

*only if Teo slept at Magda's · `wake_magdas`*

I wake up on the eleventh floor. The sun comes through a net curtain with a pattern of swans.

- *If `slept_with_magda`:*

  *→ morning after*

The blanket smells of fabric softener. There's a note on the coffee table, in very neat handwriting.

*Gone in early to do the balloons. Coffee in the pot. Lock the door and post the key back through. Thank you for last night. Magda x*

She drew a little balloon next to the x.

*→ phone check*

#### morning after

Magda's already up. I can hear her in the kitchen, humming. Humming is the worst possible sound. Humming means hope.

She comes in with two coffees in mugs. One says *WORLD'S BEST MUM*. The other has a cartoon dog on it.

**MAGDA:** I didn't know how you take it. I put sugar. Everyone wants sugar, they just don't say.

- ▸ **Kiss her good morning.** *[kind]*

  She goes pink. Twenty-one, two kids, and she goes pink.

- ▸ **Say this was a mistake.** *[cautious]*

  *(A hidden check on Teo's tact.)*

  - *If it goes badly:*

    *[Headline: Teo told Magda it was a mistake. Over her coffee.]*

    She puts the mugs down very carefully. Like they might break, or she might.

    **MAGDA:** Okay. Okay. No. You're right. Drink your coffee, Teo. It's got sugar in.

  - *Otherwise:*

    **MAGDA:** I know. I know, Teo. I'm twenty-one, not stupid.

    **MAGDA:** It was nice though. Wasn't it? Let's just say it was nice.

    **TEO:** It was nice.

    It was. That's the problem with it.

- ▸ **Say nothing. Drink the coffee.**

  I drink the coffee. It has sugar in. I don't take sugar. I drink it all.

*→ phone check*

### Wake kornels

*only if Teo slept at Kornel's · `wake_kornels`*

I wake up under a sheet with hospital corners. There's a bucket at my feet that says *BUCKET*.

- *If Teo's hangover is 3 or more (0 to 3):*

  I've used the bucket. Someone has emptied the bucket. Someone has washed the bucket and put it back. The note is new. It says *BUCKET (CLEAN)*.

Kornel is in the kitchen, in a shirt and tie already, making breakfast. Eggs. Toast cut into triangles. Coffee in the one mug, and tea for himself in a measuring jug.

**KORNEL:** Good morning. I didn't know how you like your eggs, so I made them how I like mine, and I'll have the other eggs if they're wrong.

They're not wrong. They're perfect. They're the best eggs I've ever had and I want to cry, and I think it's the hangover. I hope it's the hangover.

- ▸ **Thank him. Properly.** *[kind]*

  **TEO:** Kornel. Thank you. Really.

  He goes still, holding the spatula.

  **KORNEL:** You're welcome.

  **KORNEL:** You're very welcome, Teo.

- ▸ **Eat fast and get out.**

  I eat in four minutes. I say thanks to the door, mostly.

  **KORNEL:** Do you want a lift? It's no trouble. I'm going the same way anyway. Okay. Okay. See you there.

*→ phone check*

### Wake station

*only if Teo slept at the sobering-up station · `wake_station`*

I wake up on a plastic mattress in a room with eight beds, seven men and one drain in the middle of the floor.

The man in the next bed is wearing a suit and one shoe. He nods at me, like we're on a business trip.

**MAN:** First time?

**TEO:** Is it obvious?

**MAN:** You're looking at the drain.

At half seven, a nurse gives me my clothes in a plastic bag, still damp, and an invoice for the night. Four thousand two hundred. Breakdown: bed, supervision, "hygienic intervention." I don't want to know.

The invoice will be posted to my employer. That's the law. The nurse says it like she's telling me the weather.

**NURSE:** Have a good day, Mr Halász. Maybe less river.

*[Headline: The sobering-up station is sending the invoice to Teo's office.]*

*→ phone check*

## WEDNESDAY 09:00. THE OFFICE

### Wed arrive

*On arrival · `wed_arrive`*

The office on the morning after looks like a crime scene where everyone is both the victim and the suspect.

Grey faces. Sunglasses indoors. Someone has brought a family-sized bottle of mineral water and is drinking it with both hands.

Viktor has shaved, and been for a run, and looks fresh. Nobody understands how.

Zdenka has been here since six.

- *If Teo slept at Kornel's:*

  Kornel and I arrive together. Everyone notices. Nobody says anything.

### Wed stojan

*09:10, happens by itself · only if `stojan_promise` or `wrong_door` · `wed_stojan`*

Reception calls. There's a man asking for "Mr Permit."

- *If `told_stojan_no`:*

  Stojan's in the lobby in a suit. A real one, from a wedding a long time ago. He's not here for me. He's asking for "the woman who measures."

  He's going round me.

- *Or if `wrong_door`:*

  Stojan's in the lobby with a cake. A whole cake. Dana made it.

  **STOJAN:** She says, for the door. No hard feelings. And for the balcony. Hard feelings if not.

  The receptionist looks at the cake, then at me, then writes something down.

- *Otherwise:*

  Stojan's in the lobby holding a sausage the length of my arm, wrapped in newspaper.

  **STOJAN:** For the woman who measures. To say thank you in advance.

  **TEO:** Stojan, you can't.

  **STOJAN:** Bribe is money, Teo. Sausage is culture.

### Wed viktor

*09:15, happens by itself · `wed_viktor`*

**VIKTOR:** Teo. A moment.

- *If `viktor_promise`:*

  He closes the door. He never closes the door.

  **VIKTOR:** Last night. Good night. Good man.

  **VIKTOR:** The restructuring. It's real. September. I need people I can trust.

  **VIKTOR:** Can I trust you, Teo?

  He's not asking about the restructuring.

- *Or if Viktor remembers: saw teo with ramona:*

  He doesn't close the door. He leans on the frame, smiling.

  **VIKTOR:** Did you get home all right? Ramona said you were very good company.

  **VIKTOR:** She doesn't say that about people.

  The smile doesn't move. Nothing about him moves. He's a photograph of a man smiling.

- *Or if Viktor remembers: sabotaged:*

  He doesn't call me in. He walks past my desk and puts down a pile of files. Forty, maybe. All the drainage appeals since 2019.

  **VIKTOR:** Thought you'd like a project. You like being thorough.

  He's smiling. The office is very quiet.

- *Or if `station_invoice`:*

  He's holding an envelope. Municipal Sobering-Up Station, Kollár Street. Co-financed by the European Union.

  **VIKTOR:** Teo, Teo, Teo.

  He's delighted. He hasn't been this happy since the minister went to prison.

  **VIKTOR:** We've all been there. Well. I haven't. But people have.

- *Or if `bench_dots_connected` and `flagged_bench`:*

  **VIKTOR:** That email about the bench. Good catch. Really.

  **VIKTOR:** I've talked to the people involved. It's all in order. So you can forget about it now.

  **VIKTOR:** You can forget about it, can't you, buddy?

- *Otherwise:*

  **VIKTOR:** Good party?

  **TEO:** Good party.

  **VIKTOR:** Good. Good. Back to it.

  That's the whole conversation. I was part of the furniture of his evening, and I'm relieved.

### Magda's desk.

*Teo's choice, at: magda · takes 15 min · `wed_magda`*

The balloons are deflating. Somebody, probably Magda, has tied the saddest one to her monitor.

- *If `slept_with_magda` and `magda_hurt`:*

  She doesn't look up. She's typing. She types very fast when she's upset. It sounds like rain on a car.

  **MAGDA:** Morning, Teo.

  Just that. I didn't know she could do professional. It frightens me.

- *Or if `slept_with_magda` and `magda_hopeful`:*

  She looks up and smiles. A little smile. A secret smile, in an office where nothing has ever been a secret.

  **MAGDA:** I saved you a pastry. From the good bakery. Not the one by the tram.

  Zdenka watches us from across the room. Zdenka has already written this down somewhere.

- *Or if `slept_with_magda`:*

  She smiles at me, carefully, the way you test a step in the dark.

  **MAGDA:** Morning. Did you get home all right? I mean. You know what I mean.

- *Or if Teo slept at Magda's:*

  **MAGDA:** You found the coffee!

  She's pleased about the coffee. She's pleased I slept on her sofa. She's going to tell people. In a nice way. It'll be worse in a nice way.

- *Or if `magda_left_with_someone`:*

  She's wearing yesterday's earrings. She sees me see them.

  **MAGDA:** Don't. I had a nice time. I'm allowed a nice time.

  She is. I just wish it looked more like one.

  - *If `magda_left_with_adrian`:*

    On her desk there's a thick card, embossed. Kovál Development. Card like that is a message by itself.

- *Or if Magda remembers: sat with later:*

  **MAGDA:** Thank you. For sitting with me. At the bar.

  She says it quietly, without looking at me, and then loudly asks if anyone wants a coffee.

- *Or if Magda remembers: sat with or Magda remembers: truth:*

  **MAGDA:** Thank you. For last night. On the bench.

  She says it quietly, without looking at me, and then loudly asks if anyone wants a coffee.

- *Or if Magda remembers: left her or Magda remembers: teo left early:*

  **MAGDA:** Morning!

  Bright. Normal. She's over it, or she's decided I don't get to see if she's over it.

  She doesn't save me a pastry. There are pastries. I can see the box.

- *Or if Magda remembers: joked about her:*

  **MAGDA:** Thirty-four tickets, Teo. I used every one.

  She says it like a joke. She's turned it into a joke. That's what she does with things people do to her.

- *Otherwise:*

  **MAGDA:** Wasn't it lovely? Wasn't it a lovely party?

  It wasn't. She knows it wasn't. She's going to say it was until it was.

### Kornel.

*Teo's choice, at: kornel · takes 15 min · `wed_kornel`*

- *If `owes_kornel`:*

  **KORNEL:** About the ten euro. There's no hurry at all. I only mention it so you know I haven't forgotten.

  He hasn't forgotten. It's on a list somewhere, with a date.

- *If `kornel_friend`:*

  Kornel has put a coffee on my desk. In a mug that says *WORLD'S OKAYEST COLLEAGUE*. He bought it this morning. He must have gone to the shop on purpose.

  **KORNEL:** It's a joke. It's an ironic joke. I don't think you're okayest.

  **KORNEL:** I think you're better than okayest.

  I've got a friend. I'm thirty-two and I appear to have made a friend, and he's Kornel.

- *Or if `kornel_poem` is 2:*

  Kornel is different today. He's sitting up straighter. People keep saying "Ode to Permits" to him as they walk past, and they mean it nicely, mostly.

  - *If `kornel_first_beer`:*

    **KORNEL:** I had a beer. My first. I don't think I'll have another. I might have another.

  - *If `stood_with_kornel`:*

    **KORNEL:** You stood next to me. Nobody's ever stood next to me.

    He says it like he's reporting a fact for the minutes. Then he goes red to the ears.

- *Or if `filmed_kornel`:*

  *→ filmed*

- *Or if `kornel_poem` is 0 and `kornel_poem` has happened:*

  **KORNEL:** I've decided not to read any more poems at work. I've made a note.

  He shows me the note. It's on his hand.

  - *If `zdenka_said_it_rhymed`:*

    **KORNEL:** Zdenka said it rhymed. Zdenka doesn't say things.

- *Otherwise:*

  **KORNEL:** Good morning, Teo. Did you have a nice evening? I had an adequate evening.

#### filmed

Kornel's at his desk with his headphones on. He's not listening to anything. I can see the cable isn't plugged in.

He's seen the video. He's seen who sent the laughing emoji.

- ▸ **Apologise.** *[kind]*

  *(A hidden check on Teo's tact.)*

  - *If it goes badly:*

    **KORNEL:** It's fine. It's absolutely fine. It's a free country.

    It isn't fine.

  - *Otherwise:*

    He takes one headphone off.

    **KORNEL:** Thank you, Teo. That was a good apology. It had all the parts.

- ▸ **Pretend nothing happened.**

  I walk past. He doesn't look up. Two men pretending at each other, ten metres apart, for the rest of our careers.

### Zdenka wants a word.

*Teo's choice, at: zdenka · only if `file_seen` or `file_buried` or Zdenka remembers: asked to bury or `saw_zdenka_napkin` · takes 15 min · `wed_zdenka`*

Zdenka doesn't call me over. She just looks at me until I'm there.

- *If `file_seen`:*

  The Petrovici file is on her desk. Not on the trolley. On her desk, squared to the edge with the metal ruler.

  **ZDENKA:** It fell to the bottom of the trolley yesterday. Files are very clumsy.

  **ZDENKA:** I've filed it today, instead of Thursday. Since it was in such a hurry to be noticed.

  *[Headline: Zdenka filed the Petrovici report early. She knows why.]*

  She doesn't say anything else. She doesn't report me. Now she has something, and Zdenka keeps things.

- *Or if `file_buried`:*

  **ZDENKA:** The Petrovici file isn't on the trolley.

  **TEO:** Isn't it?

  **ZDENKA:** No. It's at the bottom of the trolley. Under eleven others. Which is not the same as on.

  She looks at me with those pale eyes.

  **ZDENKA:** I'll find it Thursday. I find everything Thursday.

- *Or if Zdenka remembers: asked to bury:*

  **ZDENKA:** Your neighbour came here this morning. Seven-fifteen. With a sausage.

  **ZDENKA:** A whole sausage, Teo. For me. As if I'm a dog at a gate.

  **ZDENKA:** I gave it to the cleaner. Tell him that.

- *Otherwise:*

  **ZDENKA:** You were looking at me. Last night. When I was folding the napkin.

  **TEO:** Was I?

  **ZDENKA:** *(smile)* People don't look at me. It's one of the good things about being me.

  She smiles. It's the first time I've seen it. I'd prefer not to see it again.

### Sit down at your desk and open your email.

*A way out · open 09:30 to 11:30 · takes 5 min · `wed_desk`*

I sit down. I open my email. There are forty-one unread messages.

## WEDNESDAY. THE END

### End viktor

*only if `hook_viktor` · `end_viktor`*

At five to twelve, an email from Viktor. Subject: *September*. No text. Just a calendar invite, for a meeting with no name, in a room I didn't know we had.

I accept it.

On the other side of the room, Magda is watching Viktor's door. Zdenka is watching Magda. Kornel is watching me. Nobody is watching Zdenka.

*→ finale*

### End ramona

*only if `hook_ramona` or `kissed_ramona` · `end_ramona`*

At twelve, my phone buzzes. A number I don't have saved.

*Bogdan will collect you Friday at 8. Wear something you'd build in. R.*

I don't know what that means. I know what it means.

*→ finale*

### End daughter

*only if `daughter_video` · `end_daughter`*

At twelve, another message from the neighbours' daughter. A screenshot of the video, paused on my face, mid-concentration, key in hand.

*its doing numbers on my story. 300 views. fix the balcony mr permit*

I'm being blackmailed by a seventeen-year-old for a toilet. This is the most important I've ever been.

*→ finale*

### End station

*only if `hook_station` or `station_invoice` · `end_station`*

By lunch, the whole office knows. Somebody's printed the invoice and put it on the noticeboard, next to the fire drill instructions.

Underneath, in Magda's handwriting: *We love you Teo!! xx*. And under that, in Kornel's: *Hygienic intervention is a normal medical procedure.*

I've never been so famous in this building, or so loved. I'd like the floor to open.

*→ finale*

### End magda

*only if `slept_with_magda` · `end_magda`*

At lunch, there's a note in my drawer. Neat handwriting. A little balloon drawn next to the x.

*Thursday? I'll cook. No pressure!! (some pressure) M x*

I put it back in the drawer. I take it out again. I put it in my wallet, next to a receipt from 2021 I keep for no reason.

*→ finale*

### End zdenka

*only if `file_filed_early` · `end_zdenka`*

At twelve, Zdenka passes my desk on the way to lunch. She leaves a pencil on it. Sharpened. The long, careful way.

I don't know what it means. I'm going to think about it for a year.

*→ finale*

### End kornel

*only if `kornel_friend` or Teo slept at Kornel's · `end_kornel`*

At twelve, Kornel appears at my desk.

**KORNEL:** Lunch? It's soup. Both soups.

**TEO:** Yeah. Yeah, all right.

He looks so happy that I have to look at the EU poster again. The family on the bridge. I'd never noticed they were holding hands.

*→ finale*

### End punished

*only if `hook_punished` · `end_punished`*

By twelve I've read four drainage appeals. There are thirty-six to go. Viktor walks past at ten to, on his way to lunch with someone from the ministry, and pats the pile like it's a dog.

**VIKTOR:** Thorough, Teo. That's what I like about you.

Magda looks at me across the office with enormous sympathy. Zdenka doesn't look at me at all, which from Zdenka is almost the same thing.

*→ finale*

### End koval

*only if `took_kovals_card` · `end_koval`*

At twelve, a call from a number with a lot of zeros in it.

**WOMAN:** Mr Halász? Kovál Development. Mr Kovál asked me to invite you to a viewing. Saturday. Riverside Residences, the show flat. Just to look.

**WOMAN:** There's no obligation. There's coffee.

I haven't said yes. She writes me down anyway.

*→ finale*

### End stojan

*only if `file_photo` or `stojan_sausage` or `told_stojan_no` or `stojan_refused` · `end_stojan`*

- *If `told_stojan_no` or `stojan_refused`:*

  At twelve, I see Stojan in the lobby again, through the glass. He's sitting very straight in the waiting area, in his wedding suit, with a folder on his knees.

  He's waiting for Zdenka. He's going to wait as long as it takes. He has the patience of a man who has built a toilet in the sky.

- *Or if `file_photo`:*

  At twelve, a message from Stojan. A photo of the balcony, from below, with a bus stop in it. He's drawn an arrow on it.

  *bus stop is very far, they exaggerate. thank you for information Teo. you are good neighbour. we will discuss*

  We will discuss. Stojan is going to discuss.

- *Otherwise:*

  At twelve, Zdenka walks past my desk carrying the sausage, wrapped in newspaper, to the cleaner.

  She doesn't say anything. She carries it like evidence.

*→ finale*

### End tower

*only if `approved_tower` · `end_tower`*

At twelve, an email from Lower Varosh. Subject: *THANK YOU!!!!*

No text. One photo. A man on the roof of his garage, standing on some scaffolding where an eleven-metre tower is going to be, waving at the camera with both arms.

I don't know if I've done a good thing. I print it out. I put it on my wall. It's the first thing I've ever put on my wall.

*→ finale*

### End poem

*only if `kornel_poem` is 2 · `end_poem`*

At twelve, I notice it on the noticeboard: *Ode to Permits*, typed, double-spaced, pinned up between the fire drill and the canteen menu.

Somebody's pinned it up with care. Four pins, one in each corner, perfectly square.

Only one person in this building uses four pins.

*→ finale*

### End video

*only if `video_in_chat` · `end_video`*

At twelve, an email to all staff from Human Resources. Subject: *Respectful Use of Group Messaging*.

It doesn't mention the video. It mentions "dignity" twice. It's signed by Viktor, who sent the second laughing emoji.

Kornel reads it at his desk. He reads it all the way through. Then he prints it out, and files it.

*→ finale*

### End consultants

*only if `played_local` · `end_consultants`*

At twelve, a LinkedIn notification. Felicity, from Brussels Advisory Partners, has tagged me in a post.

*Humbled to learn about the ancient Kaslovian tradition of drinking from a shoe. Authentic local knowledge is what co-creation is all about.*

Four hundred likes. Someone from the Ministry of Culture has commented: *Proud of our heritage!*

It's in the deck now. It's going to be in a museum by Christmas.

*→ finale*

### End default

*`end_default`*

At twelve, an email to all staff from Human Resources. Subject: *Summer Party: Feedback Form*.

Question one: *On a scale of 1 to 5, how would you rate the evening?*

I sit there for a long time with the cursor over the numbers. There should be a box for "it happened."

*→ finale*

## SHARED LINES

Passages and lines used in several places.

### On the way

*`on_the_way`*

- *If not a 55% chance:*

- *Or if it's 04:00 or later and a 50% chance:*

  *→ bakery*

- *Or if `owes_laci` and it's 01:00 or later and a 70% chance:*

  *→ laci*

- *Or if `approved_bench` and a 60% chance:*

  *→ bench*

- *Or if a 50% chance:*

  *→ euro*

- *Otherwise:*

  *→ fox*

#### bakery

On Kollár Street the bakery is already lit. A man in a white vest is pulling trays out of an oven, and the smell reaches halfway down the street.

- ▸ **Buy something.** *(only if Teo has 1 euro or more)*

  *[Money -1 euro]*

  One euro for a roll still too hot to hold. I eat it walking, from one hand to the other.

- ▸ **Ask if he's got anything going spare.** *(only if Teo has less than 1 euro)*

  He looks at me, at the shirt, at the hour. He hands me a roll from the end of a tray, a bit burnt underneath.

  **BAKER:** The burnt ones I can't sell.

  It's the best thing I've eaten all year.

- ▸ **Keep walking.**

  I keep walking. The smell follows me for a block.

#### laci

A bike comes past, slows down, comes back. It's Laci, in a hoodie, the badge still on.

**LACI:** Office man.

**LACI:** Friday, yes?

**TEO:** Friday.

He nods and rides off, standing on the pedals. Out of the bar he looks about sixteen.

#### bench

On the promenade there's a bench. Steel, a solar panel on a stalk, a USB socket. A little plaque: *Innovation Bench. Installed March 2024. Co-financed by the European Union*.

Four hundred thousand euro. I approved it this morning. It's been sitting here for over a year, waiting for my signature.

- ▸ **Sit on it.**

  I sit on it. It's a bench. My phone is on four percent, so I try the socket.

  Nothing. A small red light comes on and stays on.

- ▸ **Walk past.**

  I walk past. I don't look at it, the way you don't look at someone you've been rude about.

#### euro

A man by the tram stop asks if I can spare a euro.

- ▸ **Give him one.** *[kind] (only if Teo has 1 euro or more)*

  *[Money -1 euro]*

  I give him one. He says thank you in English, and then, to be safe, in German.

- ▸ **Show him your wallet.** *(only if Teo has less than 1 euro)*

  I open my wallet so he can see. Nothing.

  He looks in it for a while. Then he puts a euro in it.

  **STRANGER:** For the tram.

  There's no tram at this hour. I keep the euro.

  *[Money +1 euro]*

- ▸ **Say sorry, no.**

  **TEO:** Sorry.

  He says sorry too. We're both sorry.

#### fox

- *If it's 02:00 or later:*

  By the river a fox comes out from under a parked car and trots along beside me for a bit, on the other side of the road.

- *Otherwise:*

  By the river a fox comes out from under a parked car, in the last of the light, and trots along beside me for a bit.

It stops when I stop. When I look at it properly it goes back under the car.

Nobody will believe me, and I won't tell anybody anyway.

### Phone check

*`phone_check`*

- *If `kornel_poem` has happened and (`filmed_kornel` or `kornel_poem` is 0):*

  Twelve notifications. The office group chat. Someone's posted a video of Kornel's poem with the word *LEGEND*. It isn't clear which way they mean it.

  - *If `filmed_kornel`:*

    Under it, a laughing emoji from eight people. Including me, apparently, at 1:14 in the morning.

- *If `wrong_door`:*

  A message from a number I don't know. Just a video. The landing. A man trying his key in the wrong door, with enormous concentration. Then a frying pan.

  Then: *my dad says ur fixing his balcony. i'll delete this when u do x*

  *[Headline: The neighbours' daughter is holding a video of Teo hostage.]*

- *If `photo_of_viktor`:*

  In my photos: a blue, blurry picture of two shapes in a corner of Club Schengen. I look at it for a long time. I don't delete it.

- *If `owes_laci`:*

  A message from a number I don't know: *hi its laci from the bar. 8 euro friday ok? no stress*

  Somebody I owe money to. I've joined the economy.

- *If not `magda_paid` and Magda remembers: didnt pay:*

  A message from Magda, from 06:02: *dont worry about gift money!! i covered u xx*

  She covered me.

### Finale

*`finale`*

- *If `took_kovals_card`:*

  Kovál's card is still in my wallet. Thick card. Embossed. It hasn't done anything yet. Neither have I.

- *If `flagged_bench` and not `hook_bench`:*

  The Innovation Bench application is still in my inbox, approved by someone, with a note from Viktor: *Sorted. V.*

It's Wednesday. Twelve o'clock. The summer party was yesterday, and it's already a story people are telling, and I'm in it.

I'm in it.

*[Goes to: end]*

### The end screen: who Teo was today

*`today`*

- *If `shifts` more than 0:*

  Something changed in Teo today. He'll notice next week.

- *If `shame` 1 or more:*

  Teo was laughed at today. He'll laugh about it by Christmas. Maybe.

- *If `peak_drunk` 3 or more:*

  Teo was braver than he is tonight. Mostly the zhivka.

- *Otherwise:*

  - *If `peak_drunk` is 0 and `party_arrive` has happened:*

    Teo stayed sober. He saw everything. Nobody thanked him.

- *If fewer than 3 lines so far and `chose_bold` 4 or more and `chose_bold` more than `chose_cautious`:*

  Teo said things today he usually only thinks.

- *If fewer than 3 lines so far and `chose_cautious` 4 or more and `chose_cautious` more than `chose_bold`:*

  Teo waited for the right moment. It didn't come. It rarely does.

- *If fewer than 3 lines so far and `chose_kind` 3 or more:*

  Teo let people in today. Some of them will stay.

- *Or if fewer than 3 lines so far and `chose_kind` is 0:*

  Teo kept everyone at arm's length. It was quiet there.

- *If fewer than 3 lines so far and `trait_honesty` 0 or more:*

  Teo told the truth today. Someone paid for it.

- *Or if fewer than 3 lines so far:*

  Teo said what people wanted to hear. Everyone likes him. Nobody knows him.

### Viktor target

- Felicity
- the maid of honour
- the new waitress

### Use ticket

*(Note: Before the open bar, every drink costs a ticket.)*

- My last ticket. I hold on to it a second too long before I hand it over.

### Hindsight

*(Note: Hindsight: after an important check, one line names the cause. Each line once per life (two for drink).)*

- That was the zhivka talking. The zhivka has a lot to say.
- Sober me would never. Sober me wasn't invited.
- I said it because I'm the kind of man who says it. Apparently.
- I waited. I always wait. Sometimes the moment doesn't.
- I was too tired to be careful. Careful takes energy.
- It worked. I'd like to say I knew it would.
- I did everything right. It went wrong anyway. That's also a Tuesday.
