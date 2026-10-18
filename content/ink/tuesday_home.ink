// ═════════════════════════════════════════════════════════════
// TUESDAY · MORNING · Teo's flat and the stairwell
// ═════════════════════════════════════════════════════════════

=== wake
# phase: home_morning
# kind: scene
{
- mood() >= 1:
    I wake up before the alarm and feel fine. Rested. Suspiciously fine.
    Something's going to go wrong today, and I'm going to deserve it.
- mood() <= -2:
    The alarm goes off. I don't move. The ceiling has a crack shaped like Italy, or a leg.
    Thirty-two years old and I've spent a meaningful part of it identifying this crack.
- else:
    The alarm goes off. I've been awake for twenty minutes, waiting for it. Like waiting for a bus I don't want to catch.
}
Tuesday. The summer party. Viktor put it on a Tuesday so nobody drinks.
Viktor has met us, hasn't he?
STOJAN: —because it's MY balcony! #heard:through the wall
A door slams. Then the daughter's music through the wall. Just bass, and a girl singing in English about being disappointed.
- (opts)
* [Get up.]
    -> get_up
* [Check phone.]
    -> phone
* [Lie here a bit longer.]
    -> lie

= phone
Two messages.
Magda, office group: <i>PARTY TONITE!!! dont forget gift money 500 each!!! see u all xx</i>
Gift for whom? We're the gift. We're giving ourselves five hundred each.
Kornel, privately: <i>Good morning Teo. Would you like to share a taxi to the venue? I have calculated that it is cheaper.</i>
He's calculated it. Of course he has. Somewhere there's a spreadsheet with my name in it.
* [Reply: "Sure."]
    ~ qset("kornel_taxi", 1)
    ~ remember("kornel", "taxi_yes")
    ~ like("kornel", 1)
    The reply comes in four seconds. <i>Excellent. I will send you the cost breakdown.</i>
    I've made a mistake. A small, cheap, well-documented mistake.
* [Reply: "Might go straight from the office."]
    ~ qset("kornel_taxi", -1)
    ~ remember("kornel", "taxi_maybe")
    <i>Understood. The offer remains open.</i>
    Like a wound.
* [Don't reply.]
    ~ remember("kornel", "taxi_ignored")
    ~ like("kornel", -1)
    I put the phone face down. It doesn't help. Kornel can sense being read.
- -> opts

= lie
Thirty-two. When my father was thirty-two he had a wife, two kids and a Lada.
I have a ficus.
The ficus is dying. We understand each other.
* [Get up.]
    -> get_up
* {not phone} [Check phone.]
    -> phone
* {phone} [Check phone again.]
    -> phone_again

= phone_again
{ q("kornel_taxi") == 1:
    One new message. Kornel: <i>Cost breakdown attached.</i> It's a PDF. It has a logo. He's made a logo.
- else:
    Nothing new. Just the time, which has got later without asking me.
}
* [Get up.]
    -> get_up

= get_up
{ played("wake") == 0 && mood() < 0:
    I get up the way old men get out of the sea.
}
Shower. Coffee. The good shirt, because of the party, then the other shirt, because the good shirt has a story on it.
~ pass_time(20)
-> DONE


=== stairwell_stojan
# phase: stairwell_morning
# kind: scene
Stojan is on the landing in a vest, smoking under the sign that says <i>No Smoking in the Stairwell</i>. He put the sign up.
STOJAN: Teo! Mr Permit!
Nobody calls me that. Stojan calls me that.
STOJAN: Listen. Small thing. The balcony.
The "balcony" is a room now. It has a window, a roof and, since May, a toilet.
STOJAN: People from your office came. Took pictures. Very rude. Very professional. You make this go away, yes? Neighbours.
* ["I can't really do that, Stojan." #cautious]
    -> refuse
* ["Who came? Do you know their names?"]
    -> names
* ["I'll see what I can do." #kind]
    -> promise
* {mood() >= 0 || drunk() >= 1} ["What's in it for me?" #bold]
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
Zdenka. That's Zdenka. Zdenka measures everything twice. Some things three times, and nobody knows why.
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
That sentence has never once meant what it says. Everyone in this country knows it. Stojan knows it.
Stojan smiles anyway. Stojan has decided to hear "yes."
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
