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


=== party_arrive
# phase: party
# kind: enter
# priority: 100
Terasa Velka sits on the river like it's waiting for a yacht. Glass, decking, little lights strung between poles. A brass plaque by the door: <i>Co-financed by the European Union</i>.
Everything nice in this city has that plaque. If they ever put one on a person, I'll know they've made it.
Magda is at the entrance with an envelope of drink tickets, handing them out two at a time, like communion.
MAGDA: Two each! Viktor says! I'm the bank!
{ is("magda_paid"):
    She gives me two, and then, when nobody's looking, a third.
    ~ qadd("tickets_used", -1)
}
The river's going past, slow and brown and not interested.
{ is("outsider_consultants"):
    At the next table, a group in linen shirts and lanyards is having what a laminated sign calls a <i>Team Alignment Evening</i>. They're foreigners. You can tell because they're photographing the bread.
}
{ is("outsider_hen"):
    At the far end of the terrace, a hen party has taken three tables, a speaker and the moral high ground. The bride is wearing a veil and a sash that says <i>LAST NIGHT OF FREEDOM</i>. It's a Tuesday. Her freedom ends on a Saturday.
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
She's Viktor's age. White dress. Big sunglasses at a quarter to six. A handbag that, by my estimate, is a year of my salary in leather.
Viktor goes to kiss her cheek. She turns her head a perfect amount, so he gets air.
RAMONA: Darling. You didn't say it was outdoors.
VIKTOR: It's summer, Ramona.
RAMONA: I'm aware of the season.
Magda, at the ticket table, has stopped smiling. Not in a way anyone would see. In a way I see, because I'm looking at her, because I don't know where else to look.
-> DONE


=== tickets_collapse
# phase: party
# kind: interrupt
# at: 18:40
# priority: 30
At twenty to seven, the ticket system collapses.
It's Viktor who does it. Of course it is. He slaps the bar and says something to the barman, and suddenly there are trays. Trays of little glasses. Zhivka, the local plum spirit, which tastes like a grandmother's revenge.
VIKTOR: One for the team! One! It's a Tuesday!
It's never one. It's never been one in the whole history of this country.
~ flag("open_bar")
~ headline("The two-ticket rule lasted one hour and forty minutes.")
* [Take one. #drink]
    ~ have_drink(2)
    It goes down like a lit match. Everybody cheers. I cheer. I don't know what we're cheering. Viktor, probably.
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
    The barman is called Laci. He's about twenty-three and has the face of a man who's seen this office party before, in other offices.
}
{
- drunk() >= 3:
    LACI: Maybe water, boss?
    TEO: Maybe you, Laci.
    I don't know what that means. Laci doesn't either. He pours me something anyway, because tonight the city is paying.
- drunk() >= 2:
    I lean on the bar like I lean on bars in films. The bar is wetter than in films.
- else:
    LACI: What can I get you?
}
{ not is("open_bar") && tickets_left() <= 0:
    I'm out of tickets. Two tickets. It's like a ration book from the war.
}
+ [A beer. #drink]
    ~ have_drink(1)
    { not is("open_bar"):
        ~ qadd("tickets_used", 1)
    }
    Cold. Fine. Exactly what a beer is.
+ [Wine. White. #drink]
    ~ have_drink(1)
    { not is("open_bar"):
        ~ qadd("tickets_used", 1)
    }
    It's local white, from a vineyard that got EU money to stop being sour. They spent the money on a sign.
+ [Zhivka. #drink]
    ~ have_drink(2)
    { not is("open_bar"):
        ~ qadd("tickets_used", 1)
    }
    LACI: Brave.
    It's not brave. It's plum. Then it's fire. Then it's brave.
+ [Just water. #cautious]
    ~ qadd("alcohol", -1)
    Water. Laci looks at me with something like respect. Or pity. On Laci they're the same face.
- -> DONE


=== magda_early
# phase: party
# where: magda
# kind: hub
# choice: Magda, in the red dress.
# window: 17:00-20:30
# duration: 25
She's wearing it. The red dress. She looks good, genuinely, in the way people look good when they're trying too hard and it's working anyway.
{
- remembers("magda", "kind_dress"):
    MAGDA: You were right! Look! He looked at me! When I came in. He looked. #mood:happy
- remembers("magda", "honest_dress"):
    MAGDA: I wore it anyway. Like I said.
    MAGDA: He hasn't noticed. That's fine. It's early.
- remembers("magda", "cruel_dress"):
    She sees me coming and turns slightly away, to the river. She's still angry. She's earned it.
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
    I think it's a compliment. In this country, it's the best one.
* [Ask her to dance. #bold]
    ~ temp r = check("dance_magda", "nerve")
    { r:
    - 0:
        ~ humiliate(1)
        ~ remember("magda", "bad_dance")
        I dance like a man trying to get a wasp out of his shirt. Magda is very kind about it. The consultants film it.
        It'll be on a LinkedIn post by Thursday, captioned <i>local culture</i>.
    - else:
        ~ like("magda", 2)
        ~ remember("magda", "danced")
        ~ qadd("mood", 1)
        We dance. Badly, but together, and she laughs with her whole face for the first time today.
        For four minutes, nobody's looking at Viktor. Not even her.
    }
* [Steer her away from Viktor. #cautious]
    ~ remember("magda", "steered")
    I keep moving us, slowly, to the far end of the terrace, like a sheepdog with a secret.
    She doesn't notice. Or she notices and lets me. That's worse, somehow.
    { q("viktor_wingman") == 1:
        ~ like("viktor", 1)
    }
- -> DONE


=== viktor_railing
# phase: party
# where: viktor
# kind: hub
# choice: Viktor is holding court by the railing.
# window: 18:00-21:30
# duration: 25
Viktor has a cigar he doesn't light. It's a prop. It works.
He's talking to {viktor_target()}. Or, Viktor's talking and {viktor_target()} is laughing at the right moments, which is the same thing to Viktor.
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
    I laugh at the story about the minister. I laugh at the story about the boat. I do a face at the story about his first car that I didn't know I could do.
    VIKTOR: Club Schengen later. You're coming. No arguments.
    He's chosen me. I hate how good it feels.
* [Stay and watch.]
    ~ flag("saw_viktor_flirt")
    I stay. I watch. Viktor's hand finds the small of a back. It's a professional hand. It's been there before, on other backs.
* {drunk() >= 2 || nerve() >= 2} [Ruin it. Tell {viktor_target()} about Ramona. #bold]
    ~ temp r = check("sabotage_viktor", "nerve")
    TEO: Have you met his wife? She's lovely. She's just there.
    { r:
    - 0:
        ~ like("viktor", -4)
        ~ remember("viktor", "sabotaged")
        ~ headline("Teo told Viktor's date about Viktor's wife. Viktor saw.")
        ~ humiliate(1)
        {viktor_target()} just laughs. She knows. Of course she knows. Everyone knows, it's the national pastime.
        Viktor doesn't laugh. Viktor looks at me with complete calm, the way you look at a spreadsheet with an error in it.
    - else:
        ~ like("viktor", -3)
        ~ remember("viktor", "sabotaged")
        ~ flag("viktor_foiled")
        ~ headline("Teo ruined Viktor's evening. On purpose.")
        {viktor_target()} looks at the bar. At the white dress. At the handbag. Something adds up.
        She excuses herself. Viktor watches her go, and then turns to me, smiling. That's the scary part. The smile.
        VIKTOR: Good party, Teo? #mood:displeased
    }
* [Leave him to it.]
    I go. Not my business. That's what I say about most things. It's on my headstone, probably. <i>Not My Business</i>.
- -> DONE


=== magda_breaks
# phase: party
# kind: interrupt
# at: 20:15
# priority: 25
# when: not viktor_foiled
Something happens at about quarter past eight. Nobody announces it. You just feel it, like weather.
Magda finally goes over to Viktor. Red dress. Fresh lipstick. A drink for him in her hand, like an offering.
He's with {viktor_target()}. He takes the drink. He says something to Magda with his warm, generous, professional smile, the one he uses for pensioners at public consultations.
And then he turns back. Just turns back. Like closing a file.
Magda stands there with nothing in her hands.
Then she goes down the steps to the river walk, quickly, not running, because running would mean something.
~ flag("magda_crushed")
~ remember("magda", "viktor_turned_away")
-> DONE


=== magda_river
# phase: party
# where: steps
# kind: hub
# choice: Go down to the river walk, where Magda went.
# when: magda_crushed
# duration: 30
She's on a bench. Not the Innovation Bench. That doesn't exist yet. An old one, with a plaque for someone who died in 1991 and "loved this view."
Her mascara has gone. Not dramatically. Just gone, like it gave up.
MAGDA: Don't. I know. Don't. #mood:sad
MAGDA: Thirty-four tickets. I had a plan. I don't know what the plan was. Get drunk with him. And then.
MAGDA: And then what, Magda? Then nothing. Then Wednesday.
She laughs. It's a terrible laugh. It's also, a bit, funny. She knows it is. That's the worst part about Magda: she always gets the joke, even when she's the joke.
* [Sit with her. Say nothing. #kind]
    ~ like("magda", 3)
    ~ remember("magda", "sat_with")
    ~ flag("magda_walk_offer")
    I sit. The river goes past. A bottle goes past on the river. We both watch the bottle.
    MAGDA: It's going to the sea.
    TEO: It's going to a dam in Hungary.
    She laughs, properly this time. She leans on my shoulder. She smells of perfume and zhivka and something underneath, sadder, like a hospital corridor.
    MAGDA: Walk me home later? I don't want to get the tram looking like this.
* [Tell her the truth about Viktor.]
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
    }
* {drunk() >= 2} [Kiss her. #bold]
    ~ temp r2 = check("kiss_magda", "nerve")
    { r2:
    - 0:
        ~ like("magda", -1)
        ~ remember("magda", "pity_kiss_refused")
        ~ humiliate(1)
        She turns her face. Kindly. So kindly that I want to walk into the river.
        MAGDA: Oh, Teo. No. You're sweet. You're drunk and sweet.
        Sweet. Like a dog. Like a small, drunk dog.
    - else:
        ~ like("magda", 2)
        ~ remember("magda", "kissed")
        ~ flag("kissed_magda")
        ~ flag("magda_walk_offer")
        ~ headline("Teo kissed Magda by the river.")
        She kisses me back. It tastes of zhivka and lipstick and of being second choice, for both of us. We both know it. We keep going anyway.
        MAGDA: Walk me home?
    }
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
# window: 19:00-23:30
# duration: 25
Ramona is on her third glass of champagne, which she's holding by the stem like a professional.
Up close she's fifty, and has decided fifty is something that happens to other people. The work is good. Expensive. The eyes are tired in a way no clinic does.
RAMONA: You're one of his.
TEO: I work in Permits.
RAMONA: That's what I said.
She watches Viktor in the mirror behind the bar. She's been watching him in mirrors for thirty years.
RAMONA: Do you know what this bag cost?
TEO: No.
RAMONA: Four thousand euro. I bought it in March. In March he went to a conference in Vienna for four days.
RAMONA: There's a bag for every conference. I have a whole wardrobe of conferences.
* [Listen. #kind]
    ~ remember("ramona", "listened")
    ~ like("ramona", 2)
    I listen. She talks about Vienna, and about a wardrobe, and about a girl from Viktor's gym in 2009. She talks like a woman reading out the minutes of a long meeting.
    RAMONA: You're easy to talk to. Nobody talks to me at these things. They're frightened I'll tell Viktor something.
    RAMONA: I never tell Viktor anything. That's the arrangement.
    ~ flag("ramona_confided")
* {drunk() >= 2 || nerve() >= 2} [Flirt with her. #bold]
    ~ temp r = check("flirt_ramona", "tact")
    { r:
    - 0:
        ~ like("ramona", -2)
        ~ humiliate(1)
        RAMONA: Oh, darling. No.
        She pats my hand. The way you pat a dog that's brought you a sock.
    - else:
        ~ like("ramona", 3)
        ~ remember("ramona", "flirted")
        ~ flag("ramona_flirted")
        RAMONA: Careful. I'm the boss's wife. That's the most dangerous woman in any building.
        But she's smiling. Properly, with the tired eyes too.
    }
* [Ask her why she stays. #bold]
    ~ temp r2 = check("ask_ramona", "tact")
    { r2:
    - 0:
        ~ like("ramona", -3)
        ~ remember("ramona", "rude")
        RAMONA: Why do you stay in Permits?
        RAMONA: Because it's warm. Because it pays. Because it's too late to be a pilot.
        RAMONA: Same as you, darling. Same as everyone.
    - else:
        ~ like("ramona", 2)
        ~ remember("ramona", "asked_why")
        ~ flag("ramona_confided")
        She actually thinks about it. Nobody's asked her in a long time. Maybe ever.
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
# duration: 20
Zdenka has a table to herself. She's chosen it so the river's behind her and the whole party is in front, like a stage.
ZDENKA: Sit, if you're going to.
{ is("file_seen"):
    ZDENKA: The trolley is very interesting today, I hear.
    She says nothing else about it. She doesn't need to. My whole body is suddenly very interested in the tablecloth.
}
{ remembers("zdenka", "asked_to_bury"):
    ZDENKA: Your neighbour. Did you tell him I'm not for sale?
}
We watch the party. Kornel is explaining something to a potted olive tree. Magda laughs too loud at something. Viktor is being Viktor.
ZDENKA: You know what I like about parties? You find out who people are when the money's free.
* [Ask her what she's found out.]
    ZDENKA: Viktor buys a round so nobody notices he's the reason for it. Magda counts drinks so she doesn't have to count other things. Kornel wants a friend so badly it's like a smell.
    ZDENKA: And you watch. You always watch. That's not the same as being good, Teo.
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
Somebody at the hen party whoops. It's not a good whoop.
KORNEL: "In a building made of concrete, in a city by the Vel..."
* [Go up and stand beside him. In solidarity. #kind]
    ~ flag("stood_with_kornel")
    ~ remember("kornel", "stood_with")
    ~ like("kornel", 4)
    ~ temp r = check("kornel_poem_saved", "nerve")
    I walk up and stand next to him. I don't know why. My legs did it. Kornel looks at me like I've pulled him out of a well.
    { r:
    - 0:
        ~ qset("kornel_poem", 0)
        ~ humiliate(1)
        It doesn't help. Now there are two of us. A man reading a poem and a man standing next to a man reading a poem.
        Someone films it. The consultants, or the hen party, or God.
    - else:
        ~ qset("kornel_poem", 2)
        I start clapping on the rhymes. Then Magda does. Then the hen party, ironically, and then not ironically, because it's actually quite catchy, and by "...and Zdenka, who has measured all of us, and found us well," the whole terrace is cheering.
        Zdenka, for one second, looks almost moved.
        ~ headline("Kornel's poem brought the house down. Teo stood next to him.")
    }
* [Film it. #bold]
    ~ flag("filmed_kornel")
    ~ remember("kornel", "filmed")
    ~ like("kornel", -2)
    ~ qset("kornel_poem", 0)
    ~ headline("Teo filmed Kornel's poem. The video exists.")
    I film it. Everybody films it. That's the thing. If you're the only one not filming, you're the weird one.
    Kornel sees the phones. All of them. He reads to the end anyway. That's the bravest thing I've seen in years and I recorded it for laughs.
* [Look away.]
    ~ temp roll = check("kornel_poem_alone", "luck")
    ~ qset("kornel_poem", roll)
    { roll:
    - 2:
        I look at the river. Behind me, against all odds, people start clapping along. By the end, they're cheering. Kornel has never been cheered in his life. He bows too low and nearly falls.
        ~ headline("Kornel's poem was, somehow, a hit.")
    - else:
        I look at the river. It takes four minutes. They're the longest four minutes the river has ever had.
        When I turn back, Kornel's folding the A4 very small, and Viktor is clapping slowly, with a big smile, which is the cruellest thing a clap can do.
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
* [Tell them the Innovation Bench costs four hundred thousand euro.]
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
They've decided I'm the mascot. That's how hen parties work. They pick a man, any man, like choosing a goat for a sacrifice.
The bride is called Denisa. She's twenty-six, marrying a man called Robi who sells tyres, and she's having the time of her life in the way people do when the time of their life has a deadline.
DENISA: You! Office man! Are you married?
TEO: No.
DENISA: Why not? What's wrong with you? Be honest.
The whole table waits. They genuinely want to know. So do I.
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
        They're delighted. I'm the best thing that's happened to them all night. That's something. It's not much. It's something.
    - else:
        ~ qadd("mood", 1)
        ~ remember("hen", "danced")
        I dance. With seven women and a speaker, to a song about a tractor. I'm good. I'm genuinely good. Nobody from the office is watching. Of course nobody from the office is watching.
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
    I take the card. It's heavier than it looks. That's the whole point of it.
    ADRIAN: Good man. No hurry. Nothing ever needs to hurry.
* [Leave it on the bar. #cautious]
    ~ remember("koval", "refused_card")
    I leave it on the bar. He doesn't take it back. He just leaves it there, between us, like a dog that knows it'll be fed eventually.
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
I stand there and breathe. Across the water, the old housing blocks have their lights on. Every window's a life. Most of them are watching the same programme.
~ qadd("alcohol", -1)
{ mood() < 0:
    ~ qadd("mood", 1)
    I feel slightly better. Not good. Better is a long way from good, but they're on the same road.
}
-> DONE

= lean
I lean on the railing. The railing leans back.
~ temp r = check("railing", "judgment")
{ r:
- 0:
    -> over
- else:
    I hold on. The world swings, then settles, like a boat with a bad captain.
    That was nearly something. That was nearly a story people tell.
    -> DONE
}

= over
I lean a bit further. To see the lights. The lights are very beautiful from here, upside down.
Then the railing isn't there, and the river is.
~ flag("fell_in_river")
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
And then Kornel is there. Kornel, who doesn't drink, who's had three Fantas and a philosophy about each.
KORNEL: I've got you. I've got the car. Come on. Mind the step. There's a step. That was the step.
~ headline("Kornel took Teo home. To Kornel's home.")
~ goto_phase("night_kornel")
-> DONE

= drift
I walk away from it all, down to the river, to feel the air, to make the terrace stop doing that.
The river's very close now. Very friendly.
~ flag("fell_in_river")
~ headline("Teo went into the river.")
~ goto_phase("night_river")
-> DONE


// ── Ways out ─────────────────────────────────────────────────

=== leave_home
# phase: party
# where: gate
# kind: exit
# choice: Go home.
# window: 18:30-23:30
# duration: 30
{ drunk() >= 2:
    I go home the long way, by accident. The city's very beautiful at night if you don't know where you are.
- else:
    I go home. Nobody notices I've gone. That's the advantage of being me. You can leave anywhere.
}
{ played("magda_river") == 0 && is("magda_crushed"):
    On the way out, I see Magda's shoes on the steps down to the river. Just the shoes. I keep walking.
    ~ remember("magda", "teo_left_early")
}
-> DONE


=== leave_club
# phase: party
# where: viktor
# kind: exit
# choice: Go on to Club Schengen with Viktor.
# when: viktor_invite or regard(viktor) >= 5
# window: 21:00-23:30
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
# where: steps
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
# window: 21:00-23:30
# duration: 10
Ramona doesn't say goodbye to anyone. She walks to the car like she's leaving a hotel that has disappointed her.
Viktor watches her go in the mirror behind the bar. Then he sees me following her. His face doesn't change. That's how I know.
~ remember("viktor", "saw_teo_with_ramona")
~ goto_phase("night_ramona")
-> DONE


=== party_closing
# phase: party
# kind: timeout
At half eleven, the staff start stacking chairs around the last people, like a wall going up.
Laci turns the music off. In the silence, you can hear the river. It's been there the whole time.
{ drunk() >= 3 && regard("kornel") >= 0:
    KORNEL: Teo. I'm driving. I've had Fanta. Let me drive you.
    ~ goto_phase("night_kornel")
    -> DONE
}
I walk home. Somebody's left a balloon tied to a bin. Magda's balloon, from this morning. It's lasted longer than the party. It's lasted longer than most things.
-> DONE
