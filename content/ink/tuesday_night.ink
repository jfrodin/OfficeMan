// ═════════════════════════════════════════════════════════════
// TUESDAY · NIGHT · Wherever the party ends up
// ═════════════════════════════════════════════════════════════

=== club_schengen
# phase: night_club
# kind: scene
~ pass_time(30)
Club Schengen is in a former customs warehouse. The theme is "borderless." The bouncer is the biggest border I've ever seen.
He looks at me. Then at Viktor. Then he unhooks the rope, for Viktor.
Inside it's loud and blue and everyone is twenty years younger than us. Viktor moves through them like it's his living room.
VIKTOR: Teo. Teo! This is life. Look at it.
He gets a table. He gets a bottle, with a sparkler in it, carried by a girl who looks embarrassed for all of us.
{ not is("viktor_foiled"):
    -> with_target
- else:
    -> alone_with_viktor
}

= with_target
{viktor_target()} arrives twenty minutes later. Viktor stands up for her. He never stands up for anyone at work.
After a while they stop talking. After a while they're in a corner. It's none of my business, and I watch it anyway.
* [Take a photo. #bold]
    ~ flag("photo_of_viktor")
    ~ headline("Teo has a photo of Viktor that Viktor doesn't know about.")
    I don't decide to take it. My hand takes it. The flash is off. My hand thought of that too.
    It's a terrible photo. Blurry, blue, two shapes. Anyone who knows him would know him.
    I don't know what it's for. I put my phone away like it's hot.
* [Have another drink. Look away. #drink]
    ~ have_drink(2)
    I drink. I look at the dance floor. A boy is dancing with his eyes closed. He looks completely happy. I can't remember the last time I closed my eyes in public.
* [Leave.]
    I leave without saying goodbye. Viktor doesn't notice. Or he notices and it costs him nothing.
    -> home
- -> deal

= alone_with_viktor
Viktor's evening didn't go to plan. He doesn't say whose fault that is.
VIKTOR: You know your problem, Teo? You think being honest makes you good.
He pours me a drink. A big one.
VIKTOR: It doesn't. It just makes you honest.
* [Drink it. #drink]
    ~ have_drink(2)
    It burns. He watches me drink it the whole way down, like a test.
* [Leave it.]
    I leave it on the table. We both look at it.
- -> home

= deal
Later, maybe two o'clock, Viktor drops into the seat next to me with his shirt open another button. He looks fresh. He always looks fresh.
VIKTOR: I'm restructuring. After summer. Deputy head of section. Somebody who's good, somebody who's loyal.
VIKTOR: You've been loyal tonight.
He doesn't say <i>you saw nothing</i>. That's part of the job.
* [Thank him.]
    ~ flag("viktor_promise")
    ~ like("viktor", 2)
    ~ remember("viktor", "deal")
    ~ headline("Viktor half-promised Teo a promotion. In a nightclub. At two in the morning.")
    TEO: Thanks, Viktor.
    VIKTOR: Don't thank me. Earn it.
    He means keep earning it.
* [Say nothing.]
    ~ flag("viktor_promise")
    ~ remember("viktor", "deal_silent")
    I say nothing. He takes it as a yes. Everybody always takes my nothing as a yes.
- -> home

= home
~ pass_time(60)
{ q("money") >= 30:
    ~ qadd("money", -30)
    The taxi home costs thirty euro. The driver tells me about his brother in Canada the whole way, and I listen like it's the news.
- else:
    // Broke: no taxi. He walks, and Wednesday starts later and worse.
    ~ flag("walked_home")
    ~ pass_time(60)
    A taxi is thirty euro. I haven't got thirty euro. I walk.
    It takes an hour and a half. Somewhere along the river the sky goes grey, then pink, behind the sewage plant.
    -> on_the_way ->
}
-> DONE


=== magda_walk
# phase: night_magda
# kind: scene
~ pass_time(35)
Magda lives in Block C of the Florian estate. Eleven storeys. The lift works on odd days.
We walk. It's warm. She tells me about the estate when it was new, in 1983, and her mother thought it was the future.
At the door of Block C, she stops. Keys in her hand. All of them.
{
- is("kissed_magda"):
    MAGDA: Do you want to come up?
    MAGDA: You don't have to. I know what I am tonight. I'm not stupid.
- else:
    MAGDA: Thank you. Really. You didn't have to do that.
    MAGDA: Do you want a tea? Just a tea. I have a lot of tea. People give me tea.
}
* {is("kissed_magda")} [Go up.]
    -> up_together
* {not is("kissed_magda")} [Go up for tea. #kind]
    -> tea
* [Say goodnight. #cautious]
    -> goodnight

= up_together
We go up. Eleven floors, because it's an even day.
Her flat is small and very clean. There are two children's drawings on the fridge, and no children.
We're kind to each other, and careful, the way you are with someone you'll see at work in seven hours.
Afterwards she cries a little, and laughs at herself for crying, and I hold her, and I think about Viktor, which is the most disgusting thought I've ever had, and I put it away.
~ qset("slept_at", AT_MAGDAS)
~ flag("slept_with_magda")
~ remember("magda", "slept_together")
~ like("magda", 2)
~ headline("Teo spent the night at Magda's.")
-> DONE

= tea
Eleven floors, because it's an even day.
She makes tea. She shows me videos on her phone. Her son's fifth birthday, at his dad's, which she watched on a video call. Her daughter asleep in a car seat in someone else's car.
At some point I'm asleep on her sofa, and at some point she puts a blanket on me that smells of fabric softener and somebody else's life.
~ qset("slept_at", AT_MAGDAS)
~ remember("magda", "tea")
~ like("magda", 3)
~ headline("Teo fell asleep on Magda's sofa. Just that.")
-> DONE

= goodnight
TEO: Goodnight, Magda.
MAGDA: Goodnight, Teo. You're a nice man. Somebody should tell you.
She goes in. Through the glass I watch her walk past the lift, because it's an even day, and start up eleven floors in her stockings, with her shoes in her hand.
~ remember("magda", "walked_home")
~ like("magda", 1)
~ goto_phase("night_home")
-> DONE


=== ramona_car
# phase: night_ramona
# kind: scene
~ pass_time(25)
The back of Ramona's car is quieter than my flat. The driver is called Bogdan. In nineteen years he has heard everything and repeated none of it.
Ramona takes her shoes off and puts her feet on the seat like a teenager.
RAMONA: Do you know what I wanted to be? At nineteen?
RAMONA: An architect. I was going to build things. Then I married a man who decides what gets built. I thought that was the same thing.
She laughs.
RAMONA: It's not the same thing.
{ is("ramona_flirted"):
    -> close
- else:
    -> talk
}

= close
She's looking at me. The streetlights go over her face, one, two, three, like someone flicking through a book.
* [Kiss her. #bold]
    ~ temp r = check("kiss_ramona", "nerve")
    { r:
    - 0:
        ~ humiliate(1)
        ~ remember("ramona", "tried_kiss")
        She puts one finger on my lips. Not unkindly.
        RAMONA: No, darling. I'm a lot of things. I'm not a revenge.
        Bogdan's eyes in the mirror. Then not.
    - else:
        ~ flag("kissed_ramona")
        ~ remember("ramona", "kissed")
        ~ like("ramona", 2)
        ~ headline("Teo kissed his boss's wife in the back of her car.")
        She kisses me back. Whatever this is for her, it started thirty years before me.
        Bogdan turns the radio on. Very quietly. Bogdan is a professional.
    }
* [Don't. #cautious]
    ~ remember("ramona", "restraint")
    ~ like("ramona", 1)
    I look out of the window. She sees me do it.
    RAMONA: Sensible. You're very sensible, Teo. It'll keep you exactly where you are.
- -> drop

= talk
* [Tell her she still could. Build things. #kind]
    ~ like("ramona", 2)
    ~ remember("ramona", "encouraged")
    RAMONA: At fifty?
    TEO: People build things at fifty.
    RAMONA: Men build things at fifty. Women get a bag.
    But she's quiet for a while after that. Not the bad kind of quiet.
* [Ask about Viktor.]
    RAMONA: Viktor? Viktor is Viktor. I knew that when I married him.
    She holds up the handbag, as if that explains the rest.
- -> drop

= drop
The car stops outside my building. Ramona looks at it. My building. Panel block, 1979. Someone's spray-painted <i>NO</i> on the door, with nothing after it.
RAMONA: Goodnight, Teo. Don't become him.
She doesn't say which him. I don't ask.
~ remember("ramona", "dropped_home")
-> DONE


=== river_night
# phase: night_river
# kind: scene
~ pass_time(40)
The water is cold. Properly cold, all at once.
{ drunk() >= 3:
    It's very hard to tell up from down. I choose one. It's the wrong one.
}
{
- is("outsider_hen"):
    Then there are hands, a lot of them, with long nails, and a hen party screaming <i>OFFICE MAN</i>.
    They pull me out. Denisa holds my head. Someone films it.
    DENISA: You're not allowed to die! It's my hen night!
    ~ headline("A hen party pulled Teo out of the river.")
- else:
    Then there's a hand. Laci. Laci the barman, in the water up to his chest, swearing in three languages.
    LACI: Boss. Boss! Why? Why are you like this?
    ~ headline("Laci the barman pulled Teo out of the river.")
}
The police arrive, because someone called them, and then an ambulance, because someone else did. The paramedic shines a light in my eye and says I'm fine, just drunk.
The police have a solution for men who are fine and drunk.
POLICEMAN: Name?
TEO: Teo Halász. Permits.
POLICEMAN: Permits. Of course.
The Municipal Sobering-Up Station is on Kollár Street. It has a brass plaque by the door: <i>Co-financed by the European Union</i>.
I knew it. I knew they'd put one on a person.
~ qset("slept_at", AT_STATION)
~ flag("night_in_station")
~ humiliate(2)
-> DONE


=== kornel_night
# phase: night_kornel
# kind: scene
~ pass_time(30)
Kornel's car is a 2009 Dacia, brown, very clean, with an air freshener shaped like a tree that's never seen a tree.
He drives at exactly the speed limit and stops at every amber light.
KORNEL: I don't know your address. I'm going to take you to mine. I hope that's acceptable to you.
{ drunk() >= 3:
    I tell him it's acceptable. I tell him he's a great man. I tell him about my father. I don't know which parts are out loud.
}
Kornel's flat is on the ninth floor of Block F. It's tidy in a way that hurts. Books in order of height. One of everything. One mug, one plate, one chair at the table. A framed certificate: <i>Kornel Vidrascu, Regional Spelling Bee, 1996, Second Place</i>.
He makes up the sofa with sheets and hospital corners. He puts a glass of water by my head and a bucket by my feet and a note on the bucket that says <i>BUCKET</i>.
{ played("kornel_poem") > 0 && q("kornel_poem") == 2:
    KORNEL: Tonight was the best night I've had in eleven years.
    He says it to the ceiling. Then he goes to bed.
- else:
    KORNEL: Goodnight, Teo. I'm glad you're my colleague.
    He says it to the ceiling. Then he goes to bed.
}
~ qset("slept_at", AT_KORNELS)
~ remember("kornel", "teo_slept_over")
~ like("kornel", 2)
-> DONE


=== home_stairwell
# phase: night_home
# kind: scene
~ pass_time(25)
{ q("time") < 22 * 60 && drunk() <= 1:
    -> early
}
{ drunk() >= 3:
    -> very_drunk
}
-> late

= early
The stairwell at half past nine. Somebody's frying onions. The music, faint, through the door: still the disappointed girl.
{ is("stojan_promise"):
    Stojan opens his door before I reach mine. He's been listening for me.
    STOJAN: Mr Permit! Early night! Good man. So? You looked?
    { is("file_photo"):
        I show him the photo of the report. He reads it twice. He reads "bus stop" twice.
        STOJAN: Fourteen square metres. They measured. Who measures like this?
        ~ remember("stojan", "saw_report")
        ~ like("stojan", 2)
        ~ shift("honesty", -1, "first_favour")
    - else:
        TEO: I'm working on it.
        STOJAN: Of course. Of course. Working.
    }
}
I go in. I sit on the bed in my clothes.
{
- doing("stojan", "building"):
    -> next_door_building ->
- doing("stojan", "celebrating"):
    -> next_door_party ->
- else:
    Through the wall, Dana says something, and Stojan laughs, and for a moment they sound like a couple in a film. Then the moment goes.
}
~ qset("slept_at", AT_HOME)
-> DONE

= late
Stojan is on the landing again. Same vest. Same cigarette. Possibly he lives here, on the landing.
{ is("stojan_promise"):
    -> late_promised
}
STOJAN: Teo. Big night?
TEO: Medium night.
STOJAN: Medium. Ha. Medium is the worst kind. Nothing to remember.
-> bed

= late_promised
STOJAN: Mr Permit. So? You fixed?
It's the middle of the night. He's asking me about a toilet.
* ["Working on it."]
    STOJAN: Good man.
* ["Stojan, it's the middle of the night."]
    STOJAN: Problems don't sleep, Teo.
    He's right. Stojan is often right, in a terrible way.
* ["I'm not going to fix it." #bold]
    ~ remember("stojan", "teo_said_no")
    ~ like("stojan", -3)
    ~ flag("told_stojan_no")
    He looks at me for a long time. Then he stubs the cigarette out, carefully, on the No Smoking sign.
    STOJAN: Okay. Okay, Teo.
    He goes in and closes the door very quietly.
- -> bed

= very_drunk
Stojan. Stojan on the landing. Stojan is a landing.
STOJAN: Mr Permit! Big night!
I love Stojan. Why have I never told him? I need to tell him.
* ["I'll fix everything, Stojan. Everything."]
    ~ qset("stojan_promise", 1)
    ~ remember("stojan", "drunk_promise")
    ~ like("stojan", 2)
    STOJAN: This is what I'm saying! Everything!
    He hugs me. He smells of cigarettes and garlic and a kind of hope I'm going to have to deal with.
    -> bed
* [Hug Stojan.]
    ~ like("stojan", 1)
    ~ remember("stojan", "hugged")
    I hug Stojan. Stojan allows it. He pats my back twice, hard, the way you burp a baby you're not sure about.
    -> bed
* [Try your key in Stojan's door.]
    -> wrong_door

= wrong_door
The key doesn't fit. I try harder. Keys fit if you believe in them.
The door opens from the inside. It's Dana, the wife, in a dressing gown, holding a frying pan like she's done this before.
Behind her, in the hall, the daughter. Phone up. Filming.
DANA: Stojan. Your friend.
STOJAN: He's not my friend, he's from Permits.
~ flag("wrong_door")
~ remember("stojan", "wrong_door")
~ headline("Teo tried to get into the neighbours' flat. The daughter filmed it.")
~ humiliate(1)
-> bed

= bed
{ drunk() >= 3:
    I find my door. It's my door. I'm so proud of it.
}
{
- doing("stojan", "building"):
    -> next_door_building ->
- doing("stojan", "celebrating"):
    -> next_door_party ->
}
I'm asleep before I've finished lying down.
~ qset("slept_at", AT_HOME)
-> DONE

// Stojan's evening, heard through the wall (design/agendas_build.md, S1 and S2).
= next_door_building
Through the wall, a drill. Then a hammer. Then the drill again, as if it lost the argument the first time.
DANA: Stojan, it's midnight. #heard:through the wall #from:right
STOJAN: Concrete doesn't know what time it is! #heard:through the wall #from:right
He's building it anyway. Faster, so it's finished before anyone can say no to it again.
->->

= next_door_party
Through the wall, music. Accordion, then a pop song, then the accordion again, fighting it. Glasses. Somebody singing in a key that doesn't exist.
STOJAN: To Teo! To Permits! #heard:through the wall #from:right
LENKA: Some people need their sleep! For their skin! #heard:through the wall #from:right
He's celebrating. He's celebrating something I said I'd look at.
->->

// ── On the way home ──────────────────────────────────────────
// A tunnel for any walk home: sometimes something happens on the way, by chance, at most one thing.
// Which things can happen depends on the hour (evening or dawn), on money, and on the day so far.
=== on_the_way
{
- not chance("walk_something", 55):
    ->->
- q("time") >= 28 * 60 && chance("walk_bakery", 50):
    -> bakery
- is("owes_laci") && q("time") >= 25 * 60 && chance("walk_laci", 70):
    -> laci
- is("approved_bench") && chance("walk_bench", 60):
    -> bench
- chance("walk_euro", 50):
    -> euro
- else:
    -> fox
}

= bakery
On Kollár Street the bakery is already lit. A man in a white vest is pulling trays out of an oven, and the smell reaches halfway down the street.
* {q("money") >= 1} [Buy something.]
    ~ qadd("money", -1)
    One euro for a roll still too hot to hold. I eat it walking, from one hand to the other.
* {q("money") < 1} [Ask if he's got anything going spare.]
    He looks at me, at the shirt, at the hour. He hands me a roll from the end of a tray, a bit burnt underneath.
    BAKER: The burnt ones I can't sell.
    It's the best thing I've eaten all year.
* [Keep walking.]
    I keep walking. The smell follows me for a block.
- ->->

= laci
A bike comes past, slows down, comes back. It's Laci, in a hoodie, the badge still on.
LACI: Office man.
LACI: Friday, yes?
TEO: Friday.
He nods and rides off, standing on the pedals. Out of the bar he looks about sixteen.
->->

= bench
On the promenade there's a bench. Steel, a solar panel on a stalk, a USB socket. A little plaque: <i>Innovation Bench. Installed March 2024. Co-financed by the European Union</i>.
Four hundred thousand euro. I approved it this morning. It's been sitting here for over a year, waiting for my signature.
* [Sit on it.]
    I sit on it. It's a bench. My phone is on four percent, so I try the socket.
    Nothing. A small red light comes on and stays on.
* [Walk past.]
    I walk past. I don't look at it, the way you don't look at someone you've been rude about.
- ->->

= euro
A man by the tram stop asks if I can spare a euro.
* {q("money") >= 1} [Give him one. #kind]
    ~ qadd("money", -1)
    I give him one. He says thank you in English, and then, to be safe, in German.
* {q("money") < 1} [Show him your wallet.]
    I open my wallet so he can see. Nothing.
    He looks in it for a while. Then he puts a euro in it.
    STRANGER: For the tram.
    There's no tram at this hour. I keep the euro.
    ~ qadd("money", 1)
* [Say sorry, no.]
    TEO: Sorry.
    He says sorry too. We're both sorry.
- ->->

= fox
{ q("time") >= 26 * 60:
    By the river a fox comes out from under a parked car and trots along beside me for a bit, on the other side of the road.
- else:
    By the river a fox comes out from under a parked car, in the last of the light, and trots along beside me for a bit.
}
It stops when I stop. When I look at it properly it goes back under the car.
Nobody will believe me, and I won't tell anybody anyway.
->->
