// ═════════════════════════════════════════════════════════════
// TUESDAY · WORKDAY · City Planning Office, Permits Section
// ═════════════════════════════════════════════════════════════

=== office_arrive
# phase: office
# kind: enter
# priority: 100
The Permits Section is on the fourth floor of a building that was refused a permit in 1974 and built anyway.
Eleven desks, two printers that work, and a poster from the EU Cohesion Fund: a smiling family on a bridge we haven't built yet.
Zdenka has been here since six. Zdenka is always here since six.
Magda has brought balloons. Nobody asked for balloons.
Kornel waves at me like we're on opposite sides of an airport.
-> DONE


=== viktor_speech
# phase: office
# kind: interrupt
# at: 09:10
# priority: 50
# duration: 10
Viktor comes out of his office and claps once. He's never needed to clap twice.
{
- honesty() >= 0:
    One day I'll tell him that clap is ridiculous. Maybe today.
- honesty() <= -2:
    I'm already smiling. I didn't decide to. My face just does that for Viktor.
}
He's fifty, he's tanned in a way nobody in this building can afford, and his shirt is open one button further than the dress code allows. He wrote the dress code.
VIKTOR: People! Tonight. Terasa Velka, five o'clock. The city's paying, so behave.
VIKTOR: It's a Tuesday. Two drink tickets each. We're professionals.
Magda laughs as if it was a joke. Zdenka writes something down.
* [Laugh along.]
    ~ like("viktor", 1)
    I laugh, a second late. Viktor points at me anyway. Any laugh will do.
* [Say nothing.]
    I say nothing. Nobody notices that either.
* [Make a joke. #bold]
    ~ temp r = check("viktor_joke", "nerve")
    TEO: Two tickets. So Magda's got thirty.
    { r:
    - 0:
        ~ humiliate(1)
        ~ like("magda", -2)
        ~ remember("magda", "joked_about_her")
        Nobody laughs. Then Magda does, a second later and louder than anyone, and that's worse.
        Viktor looks at me as if I've parked in his space.
        VIKTOR: Let's be nice, Teo. It's a party. #mood:displeased
        He's right, and he said it in front of everyone.
    - 1:
        ~ like("viktor", 1)
        ~ remember("magda", "joked_about_her")
        ~ like("magda", -1)
        A few people laugh. Viktor, mostly. Magda smiles at the floor.
        VIKTOR: Thirty-two! She's been collecting since March!
        Magda laughs too. Then she goes quiet and straightens a balloon that was already straight.
    - 2:
        ~ like("viktor", 2)
        ~ like("magda", 1)
        ~ remember("viktor", "funny")
        It lands. Even Magda laughs, properly, and fans the tickets out of her cardigan pocket like a winning hand.
        MAGDA: Thirty-four, thank you very much!
        VIKTOR: That's my department! Look at that!
        For a moment I'm someone who says things. I don't know what to do with it.
    }
- Viktor goes back into his office and leaves the door open, so we can all see him being busy.
-> DONE


=== magda_gift
# phase: office
# kind: hub
# where: magda
# choice: Magda is collecting the gift money.
# window: 08:30-14:30
# duration: 15
Magda's desk is ready for the party: balloons, a clipboard, and a tin that says <i>GIFT</i> in glitter glue.
MAGDA: Teo! You're the last one. Well. You and Zdenka, and Zdenka said no in a way I didn't like.
{ remembers("magda", "joked_about_her"):
    She smiles the same as always. It just stops at her mouth.
}
TEO: What's the gift for?
MAGDA: For Viktor! To say thank you. For the party.
We're buying Viktor a present to thank him for a party our taxes are paying for.
* [Put in five hundred. #kind]
    ~ qadd("money", -500)
    ~ flag("magda_paid")
    ~ like("magda", 1)
    ~ remember("magda", "paid")
    MAGDA: You're a sweetheart! I'm getting him a pen. A proper one. Heavy. #mood:happy
    He'll love it. He'll sign things with it that she isn't allowed to read.
* [Put in two hundred.]
    ~ qadd("money", -200)
    ~ remember("magda", "paid_less")
    She looks at the note and doesn't say anything. She writes <i>Teo, 200</i> on the clipboard, very small and very neat.
* [Say you'll bring it later. #cautious]
    ~ remember("magda", "didnt_pay")
    MAGDA: Course. Later.
    She puts a little dot next to my name. Zdenka has a dot too.
- She lowers her voice.
MAGDA: Can I show you something? Be honest.
She turns her phone round. A red dress on a hanger, in a fitting room, photographed from above like evidence.
MAGDA: For tonight. Do you think Viktor likes red?
* [Be kind. "You'll look great." #kind]
    ~ like("magda", 2)
    ~ remember("magda", "kind_dress")
    MAGDA: You think?
    TEO: I think.
    She holds the phone against her chest for a second.
    I didn't answer her question. She didn't notice.
* [Be honest. "I don't think it's about the dress, Magda."]
    ~ temp r = check("magda_honest", "tact")
    { r:
    - 0:
        ~ like("magda", -2)
        ~ remember("magda", "cruel_dress")
        MAGDA: Oh. Okay. No, okay. #mood:sad
        She puts the phone away, face down. Then she laughs at nothing, so it looks like she's fine.
    - else:
        ~ like("magda", 1)
        ~ remember("magda", "honest_dress")
        She looks at me for a long time.
        MAGDA: I know. I know it's not.
        MAGDA: I'm wearing it anyway.
        Good for her. I mean that.
    }
* [Make it a joke. "He likes anything with a pulse." #bold]
    ~ remember("magda", "joked_viktor")
    MAGDA: Teo!
    She slaps my arm, delighted. She's taken it as good news.
    Maybe it is, for her, tonight. I don't know.
- -> DONE


=== zdenka_file
# phase: office
# kind: hub
# where: zdenka
# choice: Go and see Zdenka.
# duration: 15
Zdenka's desk has nothing on it but one file, one pen and a metal ruler. I've never seen her measure anything with the ruler.
She doesn't look up. She knows who it is from the footsteps.
ZDENKA: Teo.
{ is("zdenka_dark") && chance("zdenka_pencil", 50):
    She's sharpening a pencil with a small folding knife. Slowly. The shaving comes off in one long curl.
    She sees me watching.
    ZDENKA: Sharpeners are for people in a hurry.
}
{ is("stojan_promise") || is("stojan_asked"):
    -> petrovici
- else:
    -> small_talk
}

= small_talk
TEO: Morning, Zdenka. Coming tonight?
ZDENKA: I'm going. That's not the same as coming.
* [Ask her why she didn't pay for the gift.]
    ZDENKA: A man who earns three times my salary is getting a pen from a woman who can't afford her dentist.
    That's the longest thing she's ever said to me. I don't think it was meant for me.
    ~ like("zdenka", 1)
* [Leave her to it.]
    ZDENKA: Mm.
- -> DONE

= petrovici
TEO: The Petrovici inspection. The balcony.
ZDENKA: It is not a balcony.
TEO: No.
ZDENKA: It is a fourteen-square-metre unpermitted dwelling extension with plumbing, cantilevered over a public footpath on two steel brackets designed for air conditioning units.
ZDENKA: If it falls, it falls on a bus stop.
She says it the way other people say <i>looks like rain</i>.
* [Ask what happens next.]
    ZDENKA: I file it Thursday. Then a letter. Then a fine. He ignores the letter. Then a second letter. Then, in a year, someone dies or nobody does.
    ZDENKA: Mostly nobody does.
    ~ flag("knows_file")
* {is("stojan_promise")} [Ask if it could... take a while to file. #bold]
    ~ temp r = check("zdenka_ask", "tact")
    She looks up. Her eyes are very pale, like winter.
    { r:
    - 0:
        ~ like("zdenka", -3)
        ~ remember("zdenka", "asked_to_bury")
        ZDENKA: No.
        Just that. She goes back to her file, and I'm left standing at her desk like a coat stand.
        I'll be thinking about that no at three in the morning.
    - else:
        ~ remember("zdenka", "asked_to_bury")
        ~ like("zdenka", -1)
        ZDENKA: He's your neighbour.
        TEO: He's my neighbour.
        ZDENKA: Then he's your problem, not my file. The file is on the trolley. I file Thursday.
        That isn't a yes. She's told me where the file is, and when.
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
The trolley is where files wait. Some of them wait for years.
<i>PETROVICI, S. Unauthorised extension. Inspection: Z. Horák.</i>
There are photos. The "balcony" from below. It looks like a shed trying to escape.
And there's the bus stop.
* [Put it at the bottom of the trolley. Thursday becomes next month. #bold]
    ~ temp r = check("bury_file", "judgment")
    ~ flag("file_buried")
    ~ headline("Teo buried the Petrovici file.")
    I slide it under eleven other files. It's the most decisive thing I've done this year.
    { r == 0:
        ~ flag("file_seen")
        When I turn round, Zdenka is at the printer, facing me. The printer isn't printing.
    - else:
        Nobody saw.
        ~ shift("honesty", -1, "first_favour")
    }
* [Take a photo of the report for Stojan.]
    ~ flag("file_photo")
    ~ headline("Teo leaked an inspection report to his neighbour.")
    Click. Now Stojan will know what's coming. That's a favour, technically.
    It feels like stealing a stapler from a church.
* [Leave it where it is. #cautious]
    ~ flag("file_left")
    I put it back exactly where it was, slightly straighter.
    Stojan will find out on Thursday, the normal way.
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
    KORNEL: The cost breakdown is in your inbox. It includes a tip of ten percent. I hope that's acceptable to you.
- remembers("kornel", "taxi_ignored"):
    KORNEL: Teo. I sent you a message this morning, about a taxi. It may have gone to the wrong number.
    It didn't go to the wrong number. He's giving me a way out. He does that for people.
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
    He looks so happy that I have to look at the EU poster for a second.
* [Gently suggest he doesn't.]
    ~ temp r = check("kornel_talk", "tact")
    { r:
    - 2:
        ~ flag("kornel_talked_down")
        ~ like("kornel", 1)
        TEO: Maybe save it for a smaller crowd. Let it build.
        KORNEL: Let it build. Yes. That's very wise.
        He writes <i>let it build</i> on a Post-it and sticks it to his own hand.
    - else:
        ~ like("kornel", -1)
        ~ remember("kornel", "discouraged")
        KORNEL: I see.
        KORNEL: I'll do it anyway. But thank you for your input.
        So now he's doing it anyway, and it's brave, and that's my fault.
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
Attached is a drawing of the tower, with the man at the top, waving.
* [Refuse it. Height limit is seven metres.]
    Refused, under section 4.2. For a second I feel like a god. A small, boring god of section 4.2.
    ~ qadd("work_done", 1)
* [Ask for more information.]
    I write back asking what he means by "for the view." I actually want to know. I think about him for the rest of the morning.
    ~ qadd("work_done", 1)
* [Approve it. Why not. #bold]
    ~ flag("approved_tower")
    ~ headline("Teo approved an eleven-metre tower on a garage.")
    Approved. Somewhere in Lower Varosh a man is going to wave at the whole city.
    I don't know why I did that. It felt like freedom. It'll come back as paperwork.
- -> DONE

= bench
Application 2236. The <i>Innovation Bench</i>. EU-funded. A bench with a solar panel and a USB socket, for the river promenade.
Budget: four hundred thousand euro.
There's a consultant's report, ninety pages long. Page sixty-one says the bench will "activate public space."
* [Approve it. It's not your money.]
    As far as this building is concerned, EU money falls from the sky. Some of it lands on benches.
    ~ qadd("work_done", 1)
* [Flag the budget to Viktor. #cautious]
    ~ flag("flagged_bench")
    ~ remember("viktor", "flagged_bench")
    I write Viktor a short, careful email. I delete "frankly." I send it.
    He replies in under a minute: <i>Good catch buddy. Leave it with me.</i>
    I've never seen anything come back from Viktor's "leave it with me."
    ~ qadd("work_done", 1)
- -> DONE


=== viktor_office
# phase: office
# kind: interrupt
# at: 11:30
# priority: 40
# duration: 20
VIKTOR: Teo. In here a second.
Viktor's office has a view of the river and a framed photo of Viktor shaking hands with a minister who's now in prison. He keeps it up. He says it's history.
He sits on the edge of the desk, not behind it. He read that somewhere. It works.
VIKTOR: You're good, you know that? Quiet. But good. Zdenka says you're the only one who reads the attachments.
{ remembers("viktor", "funny"):
    VIKTOR: And funny! Who knew?
}
VIKTOR: Listen. Small thing. Tonight.
He lowers his voice. He never lowers his voice, so it isn't a small thing.
VIKTOR: Magda. Lovely girl. Lovely. But after a few drinks she gets... ideas. About me. You know.
I know. Everyone knows. The pigeons on the ledge know.
VIKTOR: And I might need to talk to some people tonight. Business. So if you could just be around her. Keep her happy. You're good with her.
VIKTOR: You'd be doing me a favour. I remember favours, Teo.
* [Agree.]
    ~ qset("viktor_wingman", 1)
    ~ remember("viktor", "wingman")
    ~ like("viktor", 2)
    VIKTOR: My man.
    He does something with his hand, as if he's going to pat my face, and pats my shoulder instead. I'm almost disappointed.
    I've just agreed to keep Magda busy so a married man can go after someone else. I didn't even pause.
* [Refuse. "That's not my job, Viktor." #cautious]
    ~ qset("viktor_wingman", -1)
    ~ remember("viktor", "refused_wingman")
    ~ like("viktor", -1)
    VIKTOR: No, no. Of course. Forget it.
    He's still smiling when I leave. He can smile and write your name in a small book at the same time.
* [Ask who he's "talking business" with. #bold]
    ~ temp r = check("viktor_ask", "nerve")
    { r:
    - 2:
        ~ remember("viktor", "asked")
        ~ like("viktor", 1)
        Viktor laughs and points at me.
        VIKTOR: See? Funny!
        He doesn't answer. Then he winks.
        ~ qset("viktor_wingman", 1)
        ~ remember("viktor", "wingman")
    - else:
        ~ like("viktor", -2)
        ~ remember("viktor", "nosy")
        VIKTOR: Business, Teo. Grown-up business.
        The room goes a degree colder.
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
Lunchtime.
* [The canteen, with Kornel. #kind]
    ~ like("kornel", 1)
    ~ remember("kornel", "lunch")
    The canteen has two options: soup, and the other soup. Kornel has both.
    KORNEL: Did you know the river is a hundred and four kilometres long? Most people think it's a hundred.
    KORNEL: I don't know why I told you that.
    I sort of like him. I'd never tell him. He'd write it down.
* [A beer at the kiosk by the tram stop. #drink]
    ~ have_drink(1)
    ~ flag("lunch_beer")
    A beer at the kiosk, standing up, next to two men from the gas board who nod at me like we're in a club. We are in a club.
    One beer, at lunch, on a Tuesday. I count it as half.
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
    I walk, along the river, past the spot where the Innovation Bench will go, if it ever does.
    The air is warm and smells of linden and diesel. It's a beautiful evening. I don't trust it.
}
-> DONE


=== office_timeout
# phase: office
# kind: timeout
# duration: 30
At half past four, Magda starts herding everyone towards the lifts.
MAGDA: Come on, come on! The city's paying!
{ q("kornel_taxi") == 1:
    Kornel's taxi is downstairs. It's been waiting since 16:25. He wanted a margin.
    ~ remember("kornel", "shared_taxi")
}
-> DONE


=== look_busy
# phase: office
# kind: hub
# where: desk
# choice: Look busy.
# repeat
# duration: 30
{ shuffle once:
-   I open a spreadsheet and look at it. Half an hour goes by. I couldn't tell you what was in it.
-   I answer an email from March. It says "Any update?" I write "Following up." That's most of the job.
-   I wash a mug in the kitchen. It isn't mine. It takes four minutes and it's the best part of the morning.
-   Down in the street a man spends twenty minutes trying to park a van. I watch all of it. When he gets it in, I nearly clap.
-   I print a form, so I can carry it to the printer and back. Walking with paper looks like work.
-   Kornel asks if I've seen the stapler. I haven't. We look for it together for a while. It's on his desk.
-   I read the minutes of a meeting I was in. I don't remember it. According to the minutes, I agreed.
-   Zdenka walks past my desk. I scroll down, with purpose.
}
{ played("look_busy") >= 8:
    I've run out of ways to look busy. I watch the screen until it goes dark, then I move the mouse.
}
-> DONE


// The dead stretch of the afternoon, skipped in one go.
=== head_down
# phase: office
# kind: hub
# where: desk
# choice: Keep your head down until it's time to go.
# window: 13:30-15:45
# duration: 0
I keep my head down. Emails. A form about another form. At some point the sun moves off my desk and onto Kornel's.
Then it's quarter to four, and people are starting to look at their coats.
~ pass_time(15 * 60 + 45 - q("time"))
-> DONE
