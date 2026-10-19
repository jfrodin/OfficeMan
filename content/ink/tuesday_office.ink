// ═════════════════════════════════════════════════════════════
// TUESDAY · WORKDAY · City Planning Office, Permits Section
// ═════════════════════════════════════════════════════════════

=== office_arrive
# phase: office
# kind: enter
# priority: 100
The Permits Section is on the fourth floor of a building that was refused a permit in 1974 and built anyway.
Eleven desks. Two working printers. A poster from the EU Cohesion Fund showing a smiling family on a bridge we haven't built yet.
Zdenka has been here since six. She's always been here since six. It's possible she was here before the building.
Magda has balloons. Nobody asked for balloons.
Kornel waves at me like we're on opposite sides of an airport.
-> DONE


=== viktor_speech
# phase: office
# kind: interrupt
# at: 09:10
# priority: 50
# duration: 10
Viktor comes out of his office and claps once. He doesn't need to clap twice. Viktor has never needed to clap twice.
{
- honesty() >= 0:
    One day I'll tell him that clap is ridiculous. Maybe today.
- honesty() <= -2:
    I'm already smiling. I didn't decide to. My face just does that for Viktor.
}
He's fifty, he's tanned in a way nobody in this building can afford, and his shirt is open one button further than the dress code. He wrote the dress code.
VIKTOR: People. Tonight. Terasa Velka, five o'clock. The city is paying, so behave like the city is watching.
VIKTOR: It's a Tuesday. Two drink tickets each. We're professionals.
Magda laughs like he's done a bit at the Christmas party. Zdenka writes something down.
* [Laugh along.]
    ~ like("viktor", 1)
    I laugh. It comes out a bit late, like a man arriving at a funeral with a cake.
    Viktor points at me anyway. Viktor loves a laugh. He doesn't check where they come from.
* [Say nothing.]
    I say nothing. Silence is my brand. Nobody's ever noticed my brand.
* [Make a joke. #bold]
    ~ temp r = check("viktor_joke", "nerve")
    TEO: Two tickets. So Magda's got thirty.
    { r:
    - 0:
        ~ humiliate(1)
        ~ like("magda", -2)
        ~ remember("magda", "joked_about_her")
        Nobody laughs. Magda laughs, after a second, the loudest of anyone, which is worse.
        Viktor looks at me like I've parked in his space.
        VIKTOR: Let's be nice, Teo. It's a party. #mood:displeased
        He's right. That's the worst part. He's always right in public.
    - 1:
        ~ like("viktor", 1)
        ~ remember("magda", "joked_about_her")
        ~ like("magda", -1)
        Some laughter. Viktor's, mostly. Magda smiles at the floor.
        VIKTOR: Thirty-two! She's been collecting since March.
        Magda laughs too. Then she goes very still, and straightens a balloon that was already straight.
    - 2:
        ~ like("viktor", 2)
        ~ like("magda", 1)
        ~ remember("viktor", "funny")
        It lands. Even Magda laughs, really laughs, and fans the tickets out of her cardigan pocket like a winning hand.
        MAGDA: Thirty-four, thank you very much.
        VIKTOR: That's my department! Look at that.
        For a moment, I'm someone who says things. I don't know what to do with it.
    }
- Viktor goes back into his office. The door stays open. It always stays open, so you can see him being busy.
-> DONE


=== magda_gift
# phase: office
# kind: hub
# where: magda
# choice: Magda is collecting the gift money.
# window: 08:30-14:30
# duration: 15
Magda's desk is a shrine to the party. Balloons. A clipboard. A tin that says <i>GIFT</i> in glitter glue.
MAGDA: Teo! You're the last one. Well. You and Zdenka, and Zdenka said no in a way I didn't like.
{ remembers("magda", "joked_about_her"):
    She's smiling. It's the same smile as normal, with something missing behind it, like a shop after hours.
}
TEO: What's the gift for?
MAGDA: For Viktor. To say thank you. For the party.
We're buying Viktor a present to thank him for the party we're paying for with our taxes.
It's perfect. It's the most Kaslovian thing that's ever happened.
* [Put in five hundred. #kind]
    ~ qadd("money", -500)
    ~ flag("magda_paid")
    ~ like("magda", 1)
    ~ remember("magda", "paid")
    MAGDA: You're a sweetheart. I'm getting him a pen. A proper one. Heavy. #mood:happy
    He'll love it. He'll use it to sign things she isn't allowed to read.
* [Put in two hundred.]
    ~ qadd("money", -200)
    ~ remember("magda", "paid_less")
    She looks at the note. She doesn't say anything. She writes <i>Teo, 200</i> on the clipboard in very small, very neat writing.
* [Say you'll bring it later. #cautious]
    ~ remember("magda", "didnt_pay")
    MAGDA: Of course. Later.
    She puts a little dot next to my name. Zdenka has a little dot too. We're the dots.
- She lowers her voice.
MAGDA: Can I show you something? Be honest.
She turns her phone. A red dress, on a hanger, in what looks like a fitting room. Taken from above, the way you'd photograph evidence.
MAGDA: For tonight. Do you think Viktor likes red?
* [Be kind. "You'll look great." #kind]
    ~ like("magda", 2)
    ~ remember("magda", "kind_dress")
    MAGDA: You think?
    TEO: I think.
    She holds the phone to her chest for a second, like it's warm.
    I didn't answer the question. She didn't notice I didn't answer the question.
* [Be honest. "I don't think it's about the dress, Magda."]
    ~ temp r = check("magda_honest", "tact")
    { r:
    - 0:
        ~ like("magda", -2)
        ~ remember("magda", "cruel_dress")
        MAGDA: Oh. Okay. No, okay. #mood:sad
        She puts the phone away face down. Then she laughs at something on her screen that isn't there.
    - else:
        ~ like("magda", 1)
        ~ remember("magda", "honest_dress")
        She looks at me for a long time.
        MAGDA: I know. I know it's not.
        MAGDA: I'm wearing it anyway.
        Good for her. Genuinely. I'd never wear anything anyway.
    }
* [Make it a joke. "He likes anything with a pulse." #bold]
    ~ remember("magda", "joked_viktor")
    MAGDA: Teo!
    She slaps my arm, delighted. She thinks it's good news.
    It might be. For her. Tonight. I genuinely don't know.
- -> DONE


=== zdenka_file
# phase: office
# kind: hub
# where: zdenka
# choice: Go and see Zdenka.
# duration: 15
Zdenka's desk has nothing on it except one file, one pen and a ruler. The ruler is metal. I've never seen her use it to measure anything.
She doesn't look up. She knows who it is from the footsteps.
ZDENKA: Teo.
{ is("zdenka_dark") && chance("zdenka_pencil", 50):
    She's sharpening a pencil with a little folding knife. Very slowly. Very evenly. The shavings come off in one long curl, like peel.
    She notices me noticing.
    ZDENKA: Sharpeners are for people in a hurry.
}
{ is("stojan_promise") || is("stojan_asked"):
    -> petrovici
- else:
    -> small_talk
}

= small_talk
TEO: Morning, Zdenka. Coming tonight?
ZDENKA: I'm going. That's different from coming.
* [Ask her why she didn't pay for the gift.]
    ZDENKA: A man with a salary three times mine is getting a pen from a woman who can't afford her dentist.
    ZDENKA: I don't contribute to weather.
    I think that's the longest sentence she's ever said to me. I think it was meant for someone else.
    ~ like("zdenka", 1)
* [Leave her to it.]
    ZDENKA: Mm.
    She's already somewhere else. A place where things are measured properly.
- -> DONE

= petrovici
TEO: The Petrovici inspection. The balcony.
ZDENKA: It is not a balcony.
TEO: No.
ZDENKA: It is a fourteen-square-metre unpermitted dwelling extension with plumbing, cantilevered over a public footpath on two steel brackets designed for air conditioning units.
ZDENKA: If it falls, it falls on a bus stop.
She says it the way other people say <i>looks like rain</i>.
* [Ask what happens next.]
    ZDENKA: I file it Thursday. Then a letter. Then a fine. Then he ignores the letter. Then a second letter. Then, in a year, someone dies or nobody does.
    ZDENKA: Mostly nobody does. That's the problem with this country. Mostly nobody does.
    ~ flag("knows_file")
* {is("stojan_promise")} [Ask if it could... take a while to file. #bold]
    ~ temp r = check("zdenka_ask", "tact")
    She finally looks up. Her eyes are very pale. Like a winter you're not dressed for.
    { r:
    - 0:
        ~ like("zdenka", -3)
        ~ remember("zdenka", "asked_to_bury")
        ZDENKA: No.
        Just that. Then she goes back to her file, and I'm standing at her desk like a coat stand.
        I'll be thinking about that "no" at three in the morning. I can already tell.
    - else:
        ~ remember("zdenka", "asked_to_bury")
        ~ like("zdenka", -1)
        ZDENKA: He's your neighbour.
        TEO: He's my neighbour.
        ZDENKA: Then it's your problem, not my file. The file is on the trolley. I file Thursday.
        It's not a yes. It's a map.
        ~ flag("knows_file")
    }
* [Leave it.]
    ZDENKA: Mm.
- -> DONE


=== petrovici_file
# phase: office
# kind: hub
# where: trolley
# choice: Find the Petrovici file on the trolley.
# when: knows_file && stojan_promise
# duration: 20
The trolley is where files go to wait. Some wait for days. Some wait so long they become history, and someone from the university comes to photograph them.
<i>PETROVICI, S. Unauthorised extension. Inspection: Z. Horák.</i>
There are photos. The "balcony" from below. It looks like a shed that's trying to escape.
And there's the bus stop.
* [Put it at the bottom of the trolley. Thursday becomes next month. #bold]
    ~ temp r = check("bury_file", "judgment")
    ~ flag("file_buried")
    ~ headline("Teo buried the Petrovici file.")
    ~ shift("honesty", -1, "first_favour")
    I slide it under eleven other files. It's the most decisive thing I've done this year.
    { r == 0:
        ~ flag("file_seen")
        When I turn round, Zdenka is at the printer. Facing me. The printer isn't printing.
    }
* [Take a photo of the report for Stojan.]
    ~ flag("file_photo")
    ~ headline("Teo leaked an inspection report to his neighbour.")
    ~ shift("honesty", -1, "first_favour")
    Click. Now Stojan will know what's coming. That's a favour, technically. That's what you do for neighbours, technically.
    It doesn't feel like a favour. It feels like I've stolen a stapler from a church.
* [Leave it where it is. #cautious]
    ~ flag("file_left")
    I put it back. Exactly where it was. Slightly straighter than it was.
    Stojan will find out on Thursday what the bus stop already knows.
- -> DONE


=== kornel_desk
# phase: office
# kind: hub
# where: kornel
# choice: Kornel is hovering near your desk.
# window: 08:30-11:15
# duration: 15
Kornel is forty-one and dresses like a man who's been told what a weekend is but has never been on one.
{
- q("kornel_taxi") == 1:
    KORNEL: Teo. I've booked the taxi for 16:40. It's a Škoda. I asked.
    KORNEL: The cost breakdown is in your inbox. It includes a tip. Ten percent. I hope that's acceptable to you.
- remembers("kornel", "taxi_ignored"):
    KORNEL: Teo. I sent you a message this morning. About a taxi. It may have gone to the wrong number.
    It didn't go to the wrong number. We both know it didn't. He's giving me a door. Kornel's always giving people doors.
- else:
    KORNEL: Teo. Are you looking forward to the event?
}
KORNEL: I've prepared something. For tonight. For the team.
TEO: Prepared what?
KORNEL: It's a surprise. But it rhymes.
Oh no.
* [Encourage him. #kind]
    ~ like("kornel", 2)
    ~ remember("kornel", "encouraged")
    ~ flag("kornel_encouraged")
    TEO: Can't wait.
    KORNEL: Really? #mood:overjoyed
    He looks so happy that I have to look at the EU poster for a second. The family on the bridge looks back. They knew this would happen.
* [Gently suggest he doesn't.]
    ~ temp r = check("kornel_talk", "tact")
    { r:
    - 2:
        ~ flag("kornel_talked_down")
        ~ like("kornel", 1)
        TEO: Maybe save it for a smaller crowd. Let it build.
        KORNEL: Let it build. Yes. Yes, that's very wise.
        He writes <i>let it build</i> on a Post-it, and sticks it to his own hand.
    - else:
        ~ like("kornel", -1)
        ~ remember("kornel", "discouraged")
        KORNEL: I see.
        KORNEL: I'll do it anyway. But thank you for your input.
        Now he's doing it anyway and I've made it worse. Now it's brave.
    }
* [Make an excuse and escape.]
    ~ remember("kornel", "escaped")
    I tell him I've got a call. I don't have a call. I sit at my desk and hold the phone to my ear like a man listening to the sea.
- -> DONE


=== work_permits
# phase: office
# kind: hub
# where: desk
# choice: Do some actual work.
# max: 2
# duration: 60
{ played("work_permits") == 0:
    -> minaret
- else:
    -> bench
}

= minaret
Application 2231. A man in Lower Varosh wants to build a decorative tower on his garage. Eleven metres. "In the style of a minaret but not religious, for the view."
Attached: a drawing of the tower, with the man at the top, waving.
* [Refuse it. Height limit is seven metres.]
    Refused. Section 4.2. I feel like a god. A small, boring god of section 4.2.
    ~ qadd("work_done", 1)
* [Ask for more information.]
    I write back asking what he means by "for the view." I genuinely want to know. I think about him for the rest of the morning.
    ~ qadd("work_done", 1)
* [Approve it. Why not. #bold]
    ~ flag("approved_tower")
    ~ headline("Teo approved an eleven-metre tower on a garage.")
    Approved. Somewhere in Lower Varosh, a man is going to wave at the whole city.
    I don't know why I did that. It felt like freedom. It's going to come back as paperwork.
- -> DONE

= bench
Application 2236. The <i>Innovation Bench</i>. EU-funded. A bench with a solar panel and a USB socket, for the river promenade.
Budget: four hundred thousand euro. For a bench.
There's a consultant's report. Ninety pages. Page sixty-one says the bench will "activate public space."
* [Approve it. It's not your money.]
    It's not anyone's money. That's what EU money is. Nobody's money, falling from the sky, onto benches.
    ~ qadd("work_done", 1)
* [Flag the budget to Viktor. #cautious]
    ~ flag("flagged_bench")
    ~ remember("viktor", "flagged_bench")
    I write Viktor a short, careful email. Then I delete "frankly." Then I send it.
    He replies in under a minute: <i>Good catch buddy. Leave it with me.</i>
    Leave it with me. That's where things go to die. Or to get rich.
    ~ qadd("work_done", 1)
- -> DONE


=== viktor_office
# phase: office
# kind: interrupt
# at: 11:30
# priority: 40
# duration: 20
VIKTOR: Teo. In here a second.
Viktor's office has a view of the river and a framed photo of himself shaking hands with a minister who's now in prison. He keeps it up. He says it's history.
He sits on the edge of the desk, not behind it. He read that somewhere. It works.
VIKTOR: You're good, you know that? Quiet. But good. Zdenka says you're the only one who reads the attachments.
{ remembers("viktor", "funny"):
    VIKTOR: And funny. Who knew? Hiding that.
}
VIKTOR: Listen. Small thing. Tonight.
He lowers his voice, which he never does, so I know it's not a small thing.
VIKTOR: Magda. She's lovely. She's a lovely woman. But she gets... after a few, she gets ideas. About me. You know.
I know. Everyone knows. The pigeons on the window ledge know.
VIKTOR: And I've got a... I might need to talk to some people tonight. Business. So if you could just. Be around her. Keep her happy. You're good with her.
VIKTOR: You'd be doing me a favour. And I remember favours, Teo.
* [Agree.]
    ~ qset("viktor_wingman", 1)
    ~ remember("viktor", "wingman")
    ~ like("viktor", 2)
    VIKTOR: My man.
    He does a thing with his hand, like he's going to pat my face, and then pats my shoulder instead. I'm almost disappointed.
    I've just agreed to babysit a woman's broken heart so a married man can go and break someone else's. And I said "sure."
* [Refuse. "That's not my job, Viktor." #cautious]
    ~ qset("viktor_wingman", -1)
    ~ remember("viktor", "refused_wingman")
    ~ like("viktor", -1)
    VIKTOR: No, no. Of course. Of course. Forget it.
    He smiles. He's still smiling when I leave. He's a man who can smile and write your name in a small book at the same time.
* [Ask who he's "talking business" with. #bold]
    ~ temp r = check("viktor_ask", "nerve")
    { r:
    - 2:
        ~ remember("viktor", "asked")
        ~ like("viktor", 1)
        Viktor laughs. Points at me.
        VIKTOR: See? Funny. Hiding that.
        He doesn't answer. Which is an answer. Then he winks, which is a confession.
        ~ qset("viktor_wingman", 1)
        ~ remember("viktor", "wingman")
    - else:
        ~ like("viktor", -2)
        ~ remember("viktor", "nosy")
        VIKTOR: Business, Teo. Grown-up business.
        The temperature in the room drops one degree. It's the most power I've ever felt someone have over a room.
        VIKTOR: Forget I asked. Go on.
    }
- -> DONE


=== lunch
# phase: office
# kind: hub
# where: door
# choice: Get some lunch.
# window: 12:00-14:00
# duration: 45
Lunch.
* [The canteen, with Kornel. #kind]
    ~ like("kornel", 1)
    ~ remember("kornel", "lunch")
    The canteen has two options: soup, and the other soup. Kornel has both.
    KORNEL: Did you know the river is a hundred and four kilometres long? Most people think a hundred.
    KORNEL: I don't know why I told you that.
    I sort of like him. I'd never tell him. He'd write it down.
* [A beer at the kiosk by the tram stop. #drink]
    ~ have_drink(1)
    ~ flag("lunch_beer")
    A beer at the kiosk. Standing up, next to two men from the gas board, who nod at me like we're in a club. We're in a club.
    One beer at lunch on a Tuesday. It's not a problem. It's a sentence that has started a lot of problems.
* [Skip it. Sit at your desk.]
    I eat a cereal bar from the drawer. It's been there since winter. It tastes like winter.
    ~ qadd("mood", -1)
- -> DONE


=== office_leave
# phase: office
# kind: exit
# where: door
# choice: Leave for the party.
# window: 15:45-16:30
# duration: 30
{ q("kornel_taxi") == 1:
    ~ remember("kornel", "shared_taxi")
    ~ like("kornel", 1)
    The taxi is a Škoda. Kornel sits in the front so he can talk to the driver about routes.
    The driver has never been happier to arrive anywhere.
- else:
    I walk. Along the river, past the place where the Innovation Bench will be, if it ever is.
    The air is warm and smells of linden and diesel. It's a beautiful evening. I don't trust it.
}
-> DONE


=== office_timeout
# phase: office
# kind: timeout
# duration: 30
At half past four, Magda starts herding everyone towards the lifts, like a sheepdog in heels.
MAGDA: Come on, come on, the city is paying!
{ q("kornel_taxi") == 1:
    Kornel's taxi is waiting downstairs. It's been waiting since 16:25. He wanted margin.
    ~ remember("kornel", "shared_taxi")
}
-> DONE


=== look_busy
# phase: office
# kind: hub
# where: desk
# choice: Look busy.
# repeat
# duration: 45
{ shuffle:
- I open a spreadsheet and look at it with the expression of a man reading a spreadsheet. Forty-five minutes pass. The spreadsheet doesn't change. Neither do I.
- I reply to an email from 2023 that says only "Any update?" I write: "Following up." Following up is the national sport. Nobody has ever won.
- I go to the kitchen and wash a mug that isn't mine. It's the most useful thing I do all day. Someone will think it was Magda.
- I stand at the window. Below, a man is trying to parallel park a van. He tries for twenty minutes. I'm with him every inch of the way.
}
-> DONE
