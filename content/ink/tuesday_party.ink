// ═════════════════════════════════════════════════════════════
// TUESDAY · EVENING · The summer party at Terasa Velka
// ═════════════════════════════════════════════════════════════

// Who Viktor has his eye on tonight depends on who else is here.
=== function viktor_target()
    { is("outsider_consultants"):
        ~ return "Felicity"
    }
    { is("outsider_hen"):
        ~ return "the maid of honour"
    }
    ~ return "the new waitress"

=== function tickets_left()
    ~ return 2 - q("tickets_used")

// Before the open bar, every drink costs a ticket.
=== function use_ticket()
    { not is("open_bar"):
        { tickets_left() == 1:
            My last ticket. I hold on to it a second too long before I hand it over.
        }
        ~ qadd("tickets_used", 1)
    }


=== party_arrive
# phase: party
# kind: enter
# priority: 100
Terasa Velka sits on the river like it's waiting for a yacht. By the door there's a brass plaque: <i>Co-financed by the European Union</i>.
Everything nice in this city has that plaque. If they ever put one on a person, I'll know they've made it.
~ set_doing("magda", "door")
~ set_doing("viktor", "railing")
~ set_doing("zdenka", "table")
Magda is at the entrance with an envelope of drink tickets, handing them out two at a time, like communion.
MAGDA: Two each! Viktor says! I'm the bank!
{ is("magda_paid"):
    She gives me two, and then, when nobody's looking, a third.
    ~ qadd("tickets_used", -1)
}
The river goes past, slow and brown.
{
- warmth() >= 2:
    Everyone's here. I like them more than I'd ever say.
- warmth() <= 0:
    Everyone's here. I've already found the exits.
}
{ is("outsider_consultants"):
    At the next table, a group in linen shirts and lanyards is having what a laminated sign calls a <i>Team Alignment Evening</i>. They're foreigners. You can tell because they're photographing the bread.
}
{ is("outsider_hen"):
    At the far end of the terrace, a hen party has taken three tables and a speaker. The bride is wearing a veil and a sash that says <i>LAST NIGHT OF FREEDOM</i>. It's Tuesday. The wedding's on Saturday.
}
{ is("outsider_developer"):
    A man in a very good suit is alone at the bar, drinking something brown and expensive, watching our party like it's a property for sale.
}
-> DONE


=== ramona_arrives
# phase: party
# kind: interrupt
# at: 17:50
# priority: 35
A black car pulls up at the kerb. The kind of black that costs extra.
Ramona Baranyi gets out like the pavement owes her money.
~ set_doing("ramona", "bar")
She's Viktor's age. White dress, big sunglasses at a quarter to six, and a handbag worth about a year of my salary.
Viktor goes to kiss her cheek. She turns her head a perfect amount, so he gets air.
RAMONA: Darling. You didn't say it was outdoors.
VIKTOR: It's summer, Ramona.
RAMONA: I'm aware of the season.
At the ticket table, Magda has stopped smiling. Nobody else would notice. I notice because I'm looking at her. I don't know where else to look.
-> DONE


=== tickets_collapse
# phase: party
# kind: interrupt
# at: 18:40
# priority: 30
At twenty to seven, the ticket system collapses.
Viktor does it. He slaps the bar, says something to Laci, and suddenly there are trays of little glasses. Zhivka: plum spirit that tastes like a grandmother's revenge.
VIKTOR: One for the team! One! It's a Tuesday!
It's never one.
~ flag("open_bar")
~ headline("The two-ticket rule lasted one hour and forty minutes.")
* [Take one. #drink]
    ~ have_drink(2)
    It goes down like a lit match. Everybody cheers, so I cheer. I don't know what for. Viktor, probably.
* [Pretend to take one. Pour it in a plant. #cautious]
    ~ flag("poured_shot")
    I tip it into a potted olive tree. Zdenka, three metres away, watches me do it.
    She raises her mineral water, very slightly. That's either respect or a warning.
* [Take two. #drink]
    ~ have_drink(4)
    ~ remember("viktor", "two_shots")
    ~ like("viktor", 1)
    VIKTOR: Look at this man!
    Second one goes down differently. Second one has opinions.
-
-> DONE


=== party_drink
# phase: party
# where: bar
# kind: hub
# choice: Get a drink.
# repeat
# duration: 15
{ played("party_drink") == 0:
    The barman is about twenty-three. His badge says LACI and under that, smaller, TRAINEE. He's done office parties before. You can tell by how he stands.
}
{ not is("open_bar") && tickets_left() <= 0:
    -> no_tickets
}
// What the bar is like depends on how drunk Teo already is.
{
- drunk() >= 3:
    { shuffle:
    -   The bar is further away than it was. I get there in the end.
        LACI: Water, boss?
        TEO: Laci. You're a good man, Laci. Has anyone told you?
        LACI: Tonight, four people.
    -   I order something. I hear myself order it, very clearly, in a voice I use for funerals.
        Laci waits. Then he puts a beer in front of me, which isn't what I asked for, but it's close.
    -   I hold on to the bar with both hands. It's important that the bar stays where it is.
        LACI: Same again?
        I nod. The nod takes a while.
    }
- drunk() >= 2:
    { shuffle:
    -   I lean on the bar the way people lean on bars in films. The bar is wetter than in films.
    -   LACI: Same again?
        I can't remember what I had. I say yes.
    -   The music's better from here. Everything's better from here. I say so to Laci. He agrees, professionally.
    }
- drunk() == 1:
    { shuffle:
    -   Laci is already reaching for a glass. He remembers me. I don't know if that's good.
    -   LACI: Back again.
        It isn't a question.
    }
- else:
    LACI: What can I get you?
}
+ [A beer. #drink]
    ~ use_ticket()
    { drunk() >= 2:
        I drink half of it standing at the bar, so I don't have to carry as much.
    - else:
        Cold. Fine. A beer.
    }
    ~ have_drink(1)
+ [Wine. White. #drink]
    ~ use_ticket()
    { played("party_drink") <= 1:
        Local white. The vineyard got EU money to stop being sour, and spent it on a sign.
    - else:
        Another white. It's getting less sour as the evening goes on, or I am.
    }
    ~ have_drink(1)
+ [Zhivka. #drink]
    ~ use_ticket()
    { drunk() >= 2:
        LACI: Another one?
        It goes down like water. That's a bad sign. I know it's a bad sign and I have another sip anyway.
    - else:
        LACI: Brave.
        It's plum, and then it's fire, and then I feel brave.
    }
    ~ have_drink(2)
+ [Just water. #cautious]
    ~ qadd("alcohol", -1)
    { drunk() >= 2:
        Water. Laci looks relieved. He's been worried about me, I realise. Laci, who I met three hours ago.
    - else:
        Water. Laci looks at me with something like respect. Or pity. On Laci they're the same face.
    }
- -> DONE

// Both tickets gone, before Viktor opens the bar.
= no_tickets
LACI: Ticket?
I haven't got one. Two tickets, and I've used them both. It's like a ration book from the war.
+ [Pay for a beer yourself. #drink]
    ~ qadd("money", -8)
    Eight euro for a beer. I pay. Laci doesn't watch me count it out, which is kind of him.
    ~ have_drink(1)
+ [Just water. #cautious]
    ~ qadd("alcohol", -1)
    Water is free. Laci gives it to me in a wine glass, so nobody can tell.
+ [Leave it.]
    I leave it. I stay at the bar anyway, so I've got somewhere to stand.
- -> DONE


=== magda_early
# phase: party
# where: magda
# kind: hub
# choice: Magda, in the red dress.
# window: 17:00-20:30
# duration: 25
She's wearing it, the red dress. She looks good. She's trying very hard, and it's working.
{
- remembers("magda", "kind_dress"):
    MAGDA: You were right! Look! He looked at me! When I came in. He looked. #mood:happy
- remembers("magda", "honest_dress"):
    MAGDA: I wore it anyway. Like I said.
    MAGDA: He hasn't noticed. That's fine. It's early.
- remembers("magda", "cruel_dress"):
    She sees me coming and turns a little towards the river. She's still angry, and she's right to be.
- else:
    MAGDA: Do I look ridiculous? Don't answer. I look ridiculous. Have a drink with me.
}
{ q("viktor_wingman") == 1:
    Over her shoulder, Viktor gives me a little nod. Keep her busy. Good boy.
}
* [Have a drink with her. #drink]
    ~ have_drink(1)
    ~ like("magda", 1)
    ~ remember("magda", "drank_together")
    We drink. She shows me photos of her kids. A boy of five, a girl of two, two different fathers. Both kids live with their dads. She says it fast, the way you pull off a plaster, and then she orders another one.
    MAGDA: You're nice to talk to, Teo. You don't do anything with your face.
    I think that's a compliment.
* [Ask her to dance. #bold]
    ~ temp r = check("dance_magda", "nerve")
    { r:
    - 0:
        ~ humiliate(1)
        ~ remember("magda", "bad_dance")
        I dance like a man trying to get a wasp out of his shirt. Magda is very kind about it.
        Somebody films it. There's always somebody.
    - else:
        ~ like("magda", 2)
        ~ remember("magda", "danced")
        ~ qadd("mood", 1)
        We dance. Badly, but together, and she laughs with her whole face for the first time today.
        For four minutes, nobody's looking at Viktor. Not even her.
    }
    {hindsight()}
* [Steer her away from Viktor. #cautious]
    ~ remember("magda", "steered")
    I keep moving us, slowly, towards the far end of the terrace.
    Either she doesn't notice, or she notices and lets me.
    { q("viktor_wingman") == 1:
        ~ like("viktor", 1)
    }
- -> DONE


=== viktor_railing
# phase: party
# where: viktor
# kind: hub
# choice: Viktor is holding court by the railing.
# when: doing(viktor, railing)
# window: 18:00-21:30
# duration: 25
Viktor has a cigar he doesn't light. It's a prop. It works.
He's talking to {viktor_target()}, and {viktor_target()} is laughing in the right places. To Viktor, that's a conversation.
Ramona is at the bar with her back to all of it, which means she can see all of it in the mirror.
{ q("viktor_wingman") == 1:
    VIKTOR: Teo! My man. Come here, come here. This is Teo, he's the brains of the department.
    And then, quietly, while {viktor_target()} looks at the river:
    VIKTOR: Where's Magda?
}
* {q("viktor_wingman") == 1} [Do your job. Laugh at his stories.]
    ~ like("viktor", 2)
    ~ flag("helped_viktor")
    ~ flag("viktor_invite")
    ~ remember("viktor", "helped")
    I laugh at the story about the minister, and the one about the boat. For the one about his first car I make a face I didn't know I had.
    VIKTOR: Club Schengen later. You're coming. No arguments.
    He's chosen me. I hate how good it feels.
* [Stay and watch.]
    ~ flag("saw_viktor_flirt")
    I stay and watch. Viktor's hand finds the small of her back. It knows the way.
* {drunk() >= 2 || nerve() >= 2} [Ruin it. Tell {viktor_target()} about Ramona. #bold]
    ~ temp r = check("sabotage_viktor", "nerve")
    TEO: Have you met his wife? She's lovely. She's just there.
    { r:
    - 0:
        ~ like("viktor", -4)
        ~ remember("viktor", "sabotaged")
        ~ headline("Teo told Viktor's date about Viktor's wife. Viktor saw.")
        ~ humiliate(1)
        {viktor_target()} just laughs. She knew. Everybody knew.
        Viktor doesn't laugh. Viktor looks at me with complete calm, the way you look at a spreadsheet with an error in it.
        ~ shift("nerve", -1, "burned_by_viktor")
    - else:
        ~ like("viktor", -3)
        ~ remember("viktor", "sabotaged")
        ~ flag("viktor_foiled")
        ~ set_doing("viktor", "bar")
        ~ feel("viktor", -2)
        ~ headline("Teo ruined Viktor's evening. On purpose.")
        {viktor_target()} looks over at the bar, at the white dress and the handbag, and does the sum.
        She excuses herself. Viktor watches her go. Then he turns to me, smiling, and the smile is the frightening part.
        VIKTOR: Good party, Teo? #mood:displeased
        ~ shift("nerve", 1, "stood_up_to_viktor")
    }
    {hindsight()}
* [Leave him to it.]
    I go. None of my business. I say that about most things.
- -> DONE


=== magda_breaks
# phase: party
# kind: interrupt
# at: 20:15
# priority: 25
# when: not viktor_foiled
At about quarter past eight, something happens. Nobody announces it.
Magda finally goes over to Viktor, in the red dress, with fresh lipstick and a drink for him in her hand.
He's with {viktor_target()}. He takes the drink. He says something to Magda with his warm, generous, professional smile, the one he uses for pensioners at public consultations.
Then he turns back to {viktor_target()}, the way you close a file.
Magda stands there with nothing in her hands.
Then she goes down the steps to the river walk, quickly, not running, because running would mean something.
~ flag("magda_crushed")
~ remember("magda", "viktor_turned_away")
~ set_doing("magda", "river")
~ feel("magda", -3)
~ qset("magda_went_down", q("time"))
-> DONE


=== magda_river
# phase: party
# where: steps
# kind: hub
# choice: Go down to the river walk, where Magda went.
# when: doing(magda, river)
# duration: 30
She's on an old bench with a plaque for someone who died in 1991 and "loved this view."
Her mascara has gone. Not run. Just gone.
MAGDA: Don't. I know. Don't. #mood:sad
MAGDA: Thirty-four tickets. I had a plan. I don't know what the plan was. Get drunk with him. And then.
MAGDA: And then what, Magda? Then nothing. Then Wednesday.
She laughs, and it's terrible, and a bit funny, and she knows it. Magda always gets the joke, even when it's on her.
* {warmth() >= 0} [Sit with her. Say nothing. #kind]
    ~ like("magda", 3)
    ~ remember("magda", "sat_with")
    ~ flag("magda_walk_offer")
    I sit. The river goes past. A bottle goes past on the river. We both watch the bottle.
    MAGDA: It's going to the sea.
    TEO: It's going to a dam in Hungary.
    She laughs, properly this time, and leans on my shoulder. She smells of perfume and zhivka.
    MAGDA: Walk me home later? I don't want to get the tram looking like this.
* {honesty() >= -1} [Tell her the truth about Viktor.]
    ~ temp r = check("magda_truth", "tact")
    TEO: He's not worth it, Magda. He's not even... He does this. He does this with everyone.
    { r:
    - 0:
        ~ like("magda", -2)
        ~ remember("magda", "lectured")
        MAGDA: Everyone. Right. Everyone but me, then.
        I've made it worse. I've made it into maths.
    - else:
        ~ like("magda", 2)
        ~ remember("magda", "truth")
        ~ flag("magda_walk_offer")
        MAGDA: I know. I know he does.
        MAGDA: But he did it to everyone else, and I wanted a turn. Isn't that stupid?
        TEO: It's not stupid.
        It is stupid. But it's the least stupid thing anyone's said tonight.
        MAGDA: Walk me home later?
        ~ shift("honesty", 1, "told_magda")
    }
    {hindsight()}
* {drunk() >= 2 || appetite() >= 1} [Kiss her. #bold]
    ~ temp r2 = check("kiss_magda", "nerve")
    { r2:
    - 0:
        ~ like("magda", -1)
        ~ remember("magda", "pity_kiss_refused")
        ~ humiliate(1)
        She turns her face. Kindly. So kindly that I want to walk into the river.
        MAGDA: Oh, Teo. No. You're sweet. You're drunk and sweet.
    - else:
        ~ like("magda", 2)
        ~ remember("magda", "kissed")
        ~ flag("kissed_magda")
        ~ flag("magda_walk_offer")
        ~ headline("Teo kissed Magda by the river.")
        She kisses me back. It tastes of zhivka and lipstick, and of both of us being somebody's second choice. We keep going anyway.
        MAGDA: Walk me home?
    }
    {hindsight()}
* [Go back up to the party.]
    ~ like("magda", -2)
    ~ remember("magda", "left_her")
    TEO: I'll get you a water.
    I don't get her a water. I go back up the steps, and it's like coming up from underwater, back into the music and the lights, and I don't go back down.
    { q("viktor_wingman") == 1:
        I was meant to keep her happy. That was my one job.
    }
- -> DONE


=== ramona_bar
# phase: party
# where: ramona
# kind: hub
# choice: Ramona is alone at the bar.
# when: played("ramona_arrives") > 0
# window: 19:00-25:00
# duration: 25
Ramona is on her third glass of champagne. She holds it by the stem.
Up close she's fifty, and has had good, expensive work done. Her eyes are tired anyway.
RAMONA: You're one of his.
TEO: I work in Permits.
RAMONA: That's what I said.
She watches Viktor in the mirror behind the bar. She's been watching him in mirrors for thirty years.
RAMONA: Do you know what this bag cost?
TEO: No.
RAMONA: Four thousand euro. I bought it in March. In March he went to a conference in Vienna for four days.
RAMONA: There's a bag for every conference. I have a whole wardrobe of conferences.
{ is("ramona_saw_viktor_go"):
    She has a bottle now, not a glass.
    RAMONA: He's gone dancing. He doesn't dance. He goes to watch other people dance and pick one.
}
* {warmth() >= 0} [Listen. #kind]
    ~ remember("ramona", "listened")
    ~ like("ramona", 2)
    I listen. She talks about Vienna, and about a wardrobe, and about a girl from Viktor's gym in 2009. She talks like a woman reading out the minutes of a long meeting.
    RAMONA: You're easy to talk to. Nobody talks to me at these things. They're frightened I'll tell Viktor something.
    RAMONA: I never tell Viktor anything. That's the arrangement.
    ~ flag("ramona_confided")
* {drunk() >= 2 || appetite() >= 1} [Flirt with her. #bold]
    ~ temp r = check("flirt_ramona", "tact")
    { r:
    - 0:
        ~ like("ramona", -2)
        ~ humiliate(1)
        RAMONA: Oh, darling. No.
        She pats my hand.
    - else:
        ~ like("ramona", 3)
        ~ remember("ramona", "flirted")
        ~ flag("ramona_flirted")
        RAMONA: Careful. I'm the boss's wife.
        But she's smiling, and this time her eyes are in it.
    }
    {hindsight()}
* [Ask her why she stays. #bold]
    ~ temp r2 = check("ask_ramona", "tact")
    { r2:
    - 0:
        ~ like("ramona", -3)
        ~ remember("ramona", "rude")
        RAMONA: Why do you stay in Permits?
        I don't have an answer. She doesn't wait for one.
    - else:
        ~ like("ramona", 2)
        ~ remember("ramona", "asked_why")
        ~ flag("ramona_confided")
        She actually thinks about it.
        RAMONA: Because I was nineteen when I met him, and he was the most alive thing in this city. And he still is. That's the problem.
        RAMONA: And because I like the bags.
        She laughs. It sounds like it's been in storage.
    }
* [Excuse yourself.]
    RAMONA: Of course. Run along.
    I run along.
- { (is("ramona_confided") || is("ramona_flirted")) && q("time") >= 1260:
    RAMONA: I'm leaving soon. Bogdan's driving. Do you want a lift? I'm not going to stay and watch the rest of this.
    ~ flag("ramona_offer_ride")
}
-> DONE


=== zdenka_table
# phase: party
# where: zdenka
# kind: hub
# choice: Zdenka is sitting alone, with mineral water.
# when: not doing(zdenka, gone)
# duration: 20
Zdenka has a table to herself. She's chosen it so the river's behind her and the whole party is in front, like a stage.
ZDENKA: Sit, if you're going to.
{ is("file_seen"):
    ZDENKA: The trolley is very interesting today, I hear.
    She says nothing else. I look at the tablecloth.
}
{ remembers("zdenka", "asked_to_bury"):
    ZDENKA: Your neighbour. Did you tell him I'm not for sale?
}
We watch. Kornel is explaining something to a potted olive tree. Behind him, Magda laughs too loud.
ZDENKA: I come every year. People drink for free, and then they tell me things.
* [Ask her what she's found out.]
    ZDENKA: Viktor buys rounds so nobody notices he's the reason for them. Kornel wants a friend so badly you can smell it.
    ZDENKA: And you watch. Watching isn't the same as being good, Teo.
    ~ like("zdenka", 1)
    ~ remember("zdenka", "talked")
* [Ask why she even came.]
    ZDENKA: Thirty-one years in that office. I've been to thirty-one of these.
    ZDENKA: I like to see everyone together. In one place. It's tidy.
    { is("zdenka_dark"):
        She folds her napkin in half. Then in half again. Then again, until it's a hard little square, which she puts in her handbag, like she'll need it later.
        ~ flag("saw_zdenka_napkin")
    }
* [Make your excuses.]
    ZDENKA: Mm.
- -> DONE


=== kornel_poem
# phase: party
# kind: interrupt
# at: 20:00
# priority: 20
# when: not kornel_talked_down
Kornel taps a glass with a fork. Then taps it again, because the first time nobody heard. Then a third time, which hits a note, and somehow that's worse.
KORNEL: Colleagues. I have written something. For the team.
He unfolds a piece of A4. Typed. Double-spaced. It has a title.
KORNEL: "Ode to Permits."
Somebody whoops. It isn't a kind whoop.
KORNEL: "In a building made of concrete, in a city by the Vel..."
* {not (nerve() <= -2 && warmth() <= -1)} [Go up and stand beside him. In solidarity. #kind]
    ~ flag("stood_with_kornel")
    ~ remember("kornel", "stood_with")
    ~ like("kornel", 4)
    ~ temp r = check("kornel_poem_saved", "nerve")
    I walk up and stand next to him. I don't know why. My legs did it. Kornel looks at me like I've pulled him out of a well.
    { r:
    - 0:
        ~ qset("kornel_poem", 0)
        ~ humiliate(1)
        It doesn't help. Now there are two of us up there.
        Someone films it.
    - else:
        ~ qset("kornel_poem", 2)
        ~ shift("warmth", 1, "stood_with_kornel")
        I start clapping on the rhymes. Then Magda does, then the next table, ironically at first and then not, because it's actually quite catchy, and by "...and Zdenka, who has measured all of us, and found us well," the whole terrace is cheering.
        Zdenka, for one second, looks almost moved.
        ~ headline("Kornel's poem brought the house down. Teo stood next to him.")
    }
    {hindsight()}
* [Film it. #bold]
    ~ flag("filmed_kornel")
    ~ remember("kornel", "filmed")
    ~ like("kornel", -2)
    ~ qset("kornel_poem", 0)
    ~ headline("Teo filmed Kornel's poem. The video exists.")
    I film it. Everybody's filming it.
    Kornel sees the phones. All of them. He reads to the end anyway. That's the bravest thing I've seen in years and I recorded it for laughs.
* [Look away.]
    ~ temp roll = check("kornel_poem_alone", "luck")
    ~ qset("kornel_poem", roll)
    { roll:
    - 2:
        I look at the river. Behind me, against all odds, people start clapping along. By the end, they're cheering. Kornel has never been cheered in his life. He bows too low and nearly falls.
        ~ headline("Kornel's poem was, somehow, a hit.")
    - else:
        I look at the river. It takes four minutes.
        When I turn back, Kornel's folding the A4 very small, and Viktor is clapping, slowly, with a big smile.
        ~ headline("Kornel read his poem. It went badly.")
    }
-
-> DONE


=== consultants_table
# phase: party
# where: consultants
# kind: hub
# choice: Join the foreigners at the Team Alignment Evening.
# when: outsider_consultants
# window: 17:30-22:30
# duration: 25
They're from Brussels Advisory Partners. They're here for three weeks to "co-create a roadmap" for the river promenade. They've been here nine days and already have a favourite local word, which is wrong.
MAARTEN: You're from here? Really from here?
TEO: Really from here.
FELICITY: Oh my God, that's amazing. What's it like? Like, actually?
They look at me like I'm a museum with a gift shop.
MAARTEN: We think there's so much potential here. The people are so... resilient.
Resilient. That's what they call you when they'd never live where you live.
* [Give them what they want. Be the authentic local. #bold]
    ~ remember("consultants", "local_act")
    ~ flag("played_local")
    ~ have_drink(1)
    I tell them about my grandmother, who fought a bear. I don't have a grandmother who fought a bear. I tell them about the old custom of drinking zhivka from a shoe. There is no such custom.
    FELICITY: That's incredible. I'm putting that in the deck.
    It's going to be in the deck. Page sixty-one. <i>Activating cultural heritage.</i>
    ~ headline("Teo invented a national custom. It's going in a consultant's deck.")
* {honesty() >= 1 || drunk() >= 1} [Tell them the Innovation Bench costs four hundred thousand euro.]
    ~ remember("consultants", "bench")
    MAARTEN: Yes, but it's not just a bench. It's a platform.
    FELICITY: It's a conversation starter.
    TEO: It's a bench.
    They look at each other the way doctors look at each other.
    { is("flagged_bench"):
        I flagged it to Viktor today. I realise Viktor is the one who invited them.
        ~ flag("bench_dots_connected")
    }
* [Let them buy you drinks. #drink]
    ~ have_drink(2)
    ~ like("consultants", 1)
    They buy a bottle of the most expensive wine on the list. It's the cheap wine with a better label. I don't tell them. That's my gift to Brussels.
- -> DONE


=== hen_party
# phase: party
# where: hen
# kind: hub
# choice: The hen party is waving you over.
# when: outsider_hen
# window: 18:00-23:00
# duration: 30
They've decided I'm their mascot. Hen parties pick a man. Tonight it's me.
The bride is called Denisa. She's twenty-six, marrying a man called Robi who sells tyres, and she's having the time of her life, with a deadline.
DENISA: You! Office man! Are you married?
TEO: No.
DENISA: Why not? What's wrong with you? Be honest.
The whole table waits. They really want to know. So do I.
* [Tell the truth. "Nobody asked."]
    ~ remember("hen", "truth")
    ~ like("hen", 2)
    DENISA: Oh my God. That's the saddest thing I ever heard. Girls! Nobody asked him!
    They all go <i>awww</i>, and one of them gives me her sash. It says <i>BRIDESMAID</i>. I wear it for the rest of the night. It's the nicest thing anyone's done for me this year.
    ~ flag("wearing_sash")
* [Do a shot with the bride. #drink]
    ~ have_drink(2)
    ~ like("hen", 1)
    We do a shot. Then another, which was apparently the same shot. Denisa tells me Robi is a good man. She tells me three times. The third time she isn't telling me.
* [Dance with them. #bold]
    ~ temp r = check("dance_hen", "nerve")
    { r:
    - 0:
        ~ humiliate(1)
        I dance. Something happens to my knee. Something else happens to a chair.
        DENISA: Office man is down!
        They're delighted. I'm the best thing that's happened to them all night.
    - else:
        ~ qadd("mood", 1)
        ~ remember("hen", "danced")
        I dance with seven women and a speaker, to a song about a tractor, and I'm good. I'm actually good. Nobody from the office sees it.
    }
- -> DONE


=== developer_bar
# phase: party
# where: developer
# kind: hub
# choice: The man in the very good suit nods at you.
# when: outsider_developer
# window: 18:00-22:30
# duration: 20
ADRIAN: Teo Halász. Permits.
He knows my name. He says it like he's reading it off a list. He's reading it off a list.
ADRIAN: Adrian Kovál. Kovál Development. Riverside Residences. You've seen the hoardings.
I've seen the hoardings. A couple drinking wine on a balcony in a building that doesn't exist yet, looking at a view that's mostly our river and partly our sewage plant.
ADRIAN: I like your department. Thorough. Mostly. Your boss and I are old friends.
He slides a card across the bar. Thick card. Embossed. Card like that is a message by itself.
ADRIAN: If you ever want to talk about your future. Or a flat with a view. We have a scheme for public servants. Very favourable.
* [Take the card. #bold]
    ~ flag("took_kovals_card")
    ~ remember("koval", "took_card")
    ~ headline("Teo took a property developer's card.")
    I take the card. It's heavier than it looks.
    ADRIAN: Good man. No hurry. Nothing ever needs to hurry.
* [Leave it on the bar. #cautious]
    ~ remember("koval", "refused_card")
    I leave it on the bar. He doesn't take it back. It stays there, between us.
    ADRIAN: Of course. Another time.
* [Ask about the Innovation Bench.]
    ADRIAN: Ha! The bench. That's a little thing. A courtesy, for the promenade. Next to my building. You understand.
    I do now.
    ~ flag("bench_dots_connected")
    ~ remember("koval", "asked_bench")
- -> DONE


=== river_air
# phase: party
# where: railing
# kind: hub
# choice: Step away to the railing for some air.
# max: 2
# duration: 20
The river smells of summer and mud, and faintly of the sewage plant, which the Riverside Residences brochure calls "the water."
{ drunk() >= 3:
    -> lean
}
I stand there and breathe. Across the water the old blocks have their lights on, most of the windows blue from the same programme.
~ qadd("alcohol", -1)
{ mood() < 0:
    ~ qadd("mood", 1)
    I feel slightly better. Not good. Better.
}
-> DONE

= lean
I lean on the railing. The railing leans back.
~ temp r = check("railing", "judgment")
{ r:
- 0:
    -> over
- else:
    I hold on. The world swings and then settles.
    That was nearly a story people tell.
    {hindsight()}
    -> DONE
}

= over
I lean a bit further. To see the lights. The lights are very beautiful from here, upside down.
Then the railing isn't there, and the river is.
~ flag("fell_in_river")
~ shift("appetite", -1, "river")
~ headline("Teo went into the river.")
~ goto_phase("night_river")
-> DONE


=== party_too_far
# phase: party
# kind: interrupt
# when: drunk >= 4 and not fell_in_river
# priority: 60
The terrace tilts. It's not the terrace.
Laci is saying something. Laci's got two faces, and both of them are worried.
{ regard("kornel") >= 2:
    -> kornel_catch
- else:
    -> drift
}

= kornel_catch
{ is("kornel_first_beer"):
    And then Kornel is there. Kornel, who had one beer, two hours ago, and has done the maths.
- else:
    And then Kornel is there. Kornel, who doesn't drink, who's had three Fantas and a philosophy about each.
}
KORNEL: I've got you. I've got the car. Come on. Mind the step. There's a step. That was the step.
~ headline("Kornel took Teo home. To Kornel's home.")
~ goto_phase("night_kornel")
-> DONE

= drift
I walk away from it all, down to the river, to feel the air, to make the terrace stop doing that.
The river's very close now. Very friendly.
~ flag("fell_in_river")
~ shift("appetite", -1, "river")
~ headline("Teo went into the river.")
~ goto_phase("night_river")
-> DONE


// ── Ways out ─────────────────────────────────────────────────

=== leave_home
# phase: party
# where: gate
# kind: exit
# choice: Go home.
# window: 18:30-25:00
# duration: 30
# repeat
// Before ten, someone may stop him at the gate. Once an evening, and he can always go anyway.
{ q("time") < 22 * 60 && not is("stopped_at_gate") && chance("stopped_at_gate", 60):
    ~ flag("stopped_at_gate")
    -> stopped
}
-> going

= going
{ drunk() >= 2:
    I go home the long way, by accident. The city's very beautiful at night if you don't know where you are.
- else:
    I go home. Nobody notices I've gone.
}
{ played("magda_river") == 0 && doing("magda", "river"):
    On the way out, I see Magda's shoes on the steps down to the river. Just the shoes. I keep walking.
    ~ remember("magda", "teo_left_early")
}
-> DONE

// Who stops him depends on who's there and what's still to come.
= stopped
{
- doing("magda", "door"):
    ~ qset("stopper", 1)
    MAGDA: You can't go! You've still got a ticket! It's against the rules to go home with a ticket.
    She says it like it's a real rule. Here, it might be.
- played("kornel_poem") == 0 && not is("kornel_talked_down"):
    ~ qset("stopper", 2)
    KORNEL: You're going? I'm reading at eight. I mean, it's nothing. It's a short thing. It rhymes.
    He's holding a piece of A4, folded in four. He's been holding it all evening.
- doing("viktor", "railing"):
    ~ qset("stopper", 3)
    VIKTOR: Teo! Nobody leaves before me. Tradition. I started it tonight.
- is("outsider_hen") && q("time") < 23 * 60:
    ~ qset("stopper", 4)
    DENISA: Office man! Where are you going? We haven't finished with you!
- else:
    -> going
}
* [Stay a bit longer.]
    ~ stay()
    ~ pass_time(5)
    { q("stopper"):
    - 1:
        ~ like("magda", 1)
        MAGDA: Good. Now you have to use it. Those are also the rules.
    - 2:
        ~ like("kornel", 1)
        KORNEL: Oh. Good. Not for the poem. Just generally. Good.
    - 3:
        ~ like("viktor", 1)
        He's already turned back to his company. He just wanted to win.
    - 4:
        They cheer. I don't know what I've agreed to.
    }
* [Go anyway.]
    { q("stopper"):
    - 1:
        ~ remember("magda", "teo_left_early")
        MAGDA: Fine. Leave me with the bank.
    - 2:
        ~ remember("kornel", "left_before_poem")
        KORNEL: No, of course. It's late. It's not late. But of course.
    - 3:
        ~ remember("viktor", "left_early")
        VIKTOR: Go on, then. Sleep is for the people who work for me.
    - 4:
        DENISA: Coward!
        She means it nicely. Mostly.
    }
    -> going
- -> DONE


=== leave_club
# phase: party
# where: viktor
# kind: exit
# choice: Go on to Club Schengen with Viktor.
# when: (viktor_invite or regard(viktor) >= 5) and not doing(viktor, club)
# window: 21:00-25:00
# duration: 20
Viktor puts an arm round me like I'm a son he chose.
VIKTOR: Come on. The night is young and we're not.
{ not is("viktor_foiled"):
    {viktor_target()} is coming too, in the car behind. Ramona, I notice, is not invited. Ramona has noticed too.
    ~ remember("ramona", "saw_teo_go_with_viktor")
}
~ goto_phase("night_club")
-> DONE


=== leave_magda
# phase: party
# where: gate
# kind: exit
# choice: Walk Magda home.
# when: magda_walk_offer
# duration: 10
Magda's carrying her shoes. I carry her bag. It weighs about the same as a small child and contains, by the sound of it, mostly keys.
~ goto_phase("night_magda")
-> DONE


=== leave_ramona
# phase: party
# where: ramona
# kind: exit
# choice: Take Ramona's lift.
# when: ramona_offer_ride
# window: 21:00-25:00
# duration: 10
Ramona doesn't say goodbye to anyone. She walks to the car like she's leaving a hotel that has disappointed her.
Viktor watches her go in the mirror behind the bar. Then he sees me following her. His face doesn't change. That's how I know.
~ remember("viktor", "saw_teo_with_ramona")
~ goto_phase("night_ramona")
-> DONE


=== party_closing
# phase: party
# kind: timeout
At one o'clock, the staff start stacking chairs around the last people, like a wall going up.
Laci turns the music off. In the silence, you can hear the river. It's been there the whole time.
{ drunk() >= 3 && regard("kornel") >= 0:
    KORNEL: Teo. I'm driving. I've had Fanta. Let me drive you.
    ~ goto_phase("night_kornel")
    -> DONE
}
I walk home. Somebody's tied a balloon to a bin. One of Magda's, from the office. It's lasted longer than the party.
-> DONE
