// ═════════════════════════════════════════════════════════════
// TUESDAY · MORNING · Teo's flat and the stairwell
// ═════════════════════════════════════════════════════════════

=== wake
# phase: home_morning
# kind: scene
// What Teo has until payday on Friday. A first guess; the creator decides pay and payday (design/noted.md).
~ qset("money", 70)
{
- mood() >= 1:
    I wake up before the alarm and I feel fine. Rested, even.
    Something's going to go wrong today, and I'm going to deserve it.
- mood() <= -2:
    The alarm goes off. I don't move.
    There's a crack in the ceiling shaped like Italy, or a leg. Six years I've lived here and I still haven't decided.
- else:
    The alarm goes off. I've been awake for twenty minutes, waiting for it.
}
Tuesday. The office summer party is tonight. Viktor put it on a Tuesday so that nobody would get too drunk.
// How Teo goes into the day. It leans him one way until Tuesday is over (vow_<trait>, see Teo.cs);
// the evening can remind him of it.
* [A sober Tuesday. I'll fucking show him. #drink]
    ~ qset("vow", 1)
    ~ qset("vow_appetite", 1)
* [Two drinks. Home by nine. #cautious]
    ~ qset("vow", 2)
    ~ qset("vow_appetite", -1)
* [This year I'm not standing by the crisps all night. #bold]
    ~ qset("vow", 3)
    ~ qset("vow_nerve", 1)
-
STOJAN: Because it's MY balcony! #heard:through the wall #from:right
A door slams. Then the daughter's music through the wall: bass, and a girl singing in English about being disappointed.
- (opts)
* [Get up.]
    -> get_up
* {not phone} [Check phone.]
    -> phone
* {phone and not phone_again} [Check phone again.]
    -> phone_again
* {not lie} [Stay in bed a bit longer.]
    -> lie

= phone
Two messages.
Magda, to the office group: <i>PARTY TONITE!!! dont forget gift money 10 each!!! see u all xx</i>
Gift for who? Nobody's leaving. Nobody's getting married. Magda will have a reason by lunch.
I open the bank app with one eye shut. <i>70.00 EUR</i>. Payday is Friday.
Kornel, just to me: <i>Good morning Teo. Would you like to share a taxi to the venue? I have calculated that it is cheaper.</i>
He's calculated it. There's a spreadsheet somewhere with my name on it.
* [Reply: "Sure."]
    ~ qset("kornel_taxi", 1)
    ~ remember("kornel", "taxi_yes")
    ~ like("kornel", 1)
    The answer comes in four seconds. He was holding the phone.
    <i>Excellent. I will send you the cost breakdown.</i>
* [Reply: "Might go straight from the office."]
    ~ qset("kornel_taxi", -1)
    ~ remember("kornel", "taxi_maybe")
    <i>Understood. The offer remains open.</i>
* [Don't reply.]
    ~ remember("kornel", "taxi_ignored")
    ~ like("kornel", -1)
    I put the phone face down. He'll have seen the two blue ticks by now.
- -> opts

= lie
Thirty-two. When my father was thirty-two he had a wife, two kids and a Lada.
I have a ficus.
The ficus is dying. We understand each other.
-> opts

= phone_again
{ q("kornel_taxi") == 1:
    One new message from Kornel. <i>Cost breakdown attached.</i>
    It's a PDF. He's put a logo on it.
- else:
    Nothing new. It's just later than it was.
}
-> opts

= get_up
{ played("wake") == 0 && mood() < 0:
    I get up the way old men get out of the sea.
}
Shower. Coffee. I put on the good shirt for the party, then take it off again. The good shirt has a story on it.
~ pass_time(20)
-> DONE


// Up and dressed, Teo can walk round the flat (content/rooms/flat.json: things to look at) before he goes.
=== flat_leave
# phase: home_up
# kind: exit
# where: door
# choice: Go to work.
# duration: 5
Keys, wallet, phone. I check the hob is off. It's always off. I check it anyway.
-> DONE


// The landing is a room Teo walks through. Stojan calls out when he comes near (content/rooms/stairwell.json,
// "hail"); going over to him is Teo's choice. Walking on past him is the other one (stairwell_down).
=== stairwell_stojan
# phase: stairwell_morning
# kind: hub
# where: stojan
# choice: Stojan.
# duration: 10
Stojan is on the landing in a vest, smoking under the sign that says <i>No Smoking in the Stairwell</i>. He put the sign up.
STOJAN: Teo! Mr Permit!
Nobody calls me that. Stojan calls me that.
{
- nerve() >= 0:
    I could tell him no. I won't. But I could.
- nerve() <= -2:
    I already know I'm going to say yes to whatever this is.
}
STOJAN: Listen. Small thing. The balcony.
The "balcony" is a room now. It has a window, a roof and, since May, a toilet.
STOJAN: People from your office came. Took pictures. Very rude. Very professional. You make this go away, yes? Neighbours.
* ["I can't really do that, Stojan." #cautious]
    -> refuse
* ["Who came? Do you know their names?"]
    -> names
* ["I'll see what I can do." #kind]
    -> promise
* {mood() >= 0 || drunk() >= 1 || nerve() >= 0} ["What's in it for me?" #bold]
    -> bargain

= refuse
~ qset("stojan_refused", 1)
~ remember("stojan", "refused")
~ like("stojan", -2)
STOJAN: Of course. Of course. Rules.
He nods as if I've said something wise. Then he looks at my door for slightly too long, like a man memorising a door.
-> DONE

= names
~ qset("stojan_asked", 1)
~ remember("stojan", "asked_names")
STOJAN: A woman. Older. Grey suit. She measured everything twice and said nothing. Not even good morning.
Zdenka. She measures everything twice. Some things three times, and nobody knows which.
STOJAN: You know her?
* ["She's good. She'll be fair."]
    STOJAN: Fair. Fair is for football.
* ["I'll have a word."]
    -> promise
- He flicks ash at the sign.
-> DONE

= promise
~ qset("stojan_promise", 1)
~ remember("stojan", "promised")
~ like("stojan", 2)
In this country that sentence has never once meant yes. Stojan knows that as well as I do.
He smiles anyway. He's decided to hear it.
STOJAN: Good man. I knew. I said to Dana, Teo is a good man. Quiet, but good.
Through the wall, Dana says something that doesn't sound like agreement.
-> DONE

= bargain
~ qset("stojan_promise", 1)
~ qset("stojan_bargain", 1)
~ remember("stojan", "promised")
~ remember("stojan", "bargained")
~ like("stojan", 1)
Stojan laughs, a big one. He points his cigarette at the wall.
STOJAN: You fix this, you can marry my daughter.
TEO: She's seventeen, Stojan.
STOJAN: So you have one year to fix it. I'm a patient man.
Through the wall, something breaks. Stojan doesn't flinch.
He's a patient man.
-> DONE


=== stairwell_down
# phase: stairwell_morning
# kind: exit
# where: stairs
# choice: Go down to work.
# duration: 5
{ played("stairwell_stojan") == 0:
    ~ remember("stojan", "walked_past")
    ~ like("stojan", -1)
    STOJAN: Later, Mr Permit! Later we talk! #heard:behind me
    I take the stairs two at a time, like a man with somewhere to be.
- else:
    I take the stairs down. Four floors. The lift has been "nearly fixed" since March.
}
-> DONE
