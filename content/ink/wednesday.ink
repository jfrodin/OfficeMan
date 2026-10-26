// ═════════════════════════════════════════════════════════════
// WEDNESDAY · MORNING · Where Tuesday comes back
// ═════════════════════════════════════════════════════════════

=== function hangover()
    ~ return q("peak_drunk")


// ── Waking up ────────────────────────────────────────────────

=== wake_home
# phase: wed_wake
# kind: scene
# when: slept_at == 1 or slept_at == 0
{
- hangover() >= 3:
    I wake up because my mouth has become a carpet. Somebody else's carpet. From a bar.
    My shoes are on. One of them is on the wrong foot, which shouldn't be possible, and yet.
- hangover() >= 2:
    I wake up at ten past seven with a headache shaped like Viktor.
- else:
    I wake up at ten past seven, rested, which feels like cheating. Like I skipped something everyone else had to sit through.
}
{ hungover():
    Everything is too bright and slightly my fault.
}
{ is("wearing_sash"):
    I'm still wearing the sash. <i>BRIDESMAID</i>. I leave it on for a while. Nobody can see me.
}
{ is("kissed_ramona"):
    I lie there and remember the car. The leather. The radio, very quietly.
    I kissed my boss's wife. On a Tuesday. With the driver watching.
    I put the pillow over my face. It doesn't help. The pillow knows too.
}
-> phone_check


=== wake_magdas
# phase: wed_wake
# kind: scene
# when: slept_at == 2
# scene: magda_flat
I wake up on the eleventh floor. The sun comes through a net curtain with a pattern of swans.
{ is("slept_with_magda"):
    -> morning_after
}
The blanket smells of fabric softener. There's a note on the coffee table, in very neat handwriting.
<i>Gone in early to do the balloons. Coffee in the pot. Lock the door and post the key back through. Thank you for last night. Magda x</i>
She drew a little balloon next to the x.
-> phone_check

= morning_after
Magda's already up. I can hear her in the kitchen, humming. Humming is the worst possible sound. Humming means hope.
She comes in with two coffees in mugs. One says <i>WORLD'S BEST MUM</i>. The other has a cartoon dog on it.
MAGDA: I didn't know how you take it. I put sugar. Everyone wants sugar, they just don't say.
* [Kiss her good morning. #kind]
    ~ remember("magda", "morning_kiss")
    ~ like("magda", 2)
    ~ flag("magda_hopeful")
    She goes pink. Twenty-one, two kids, and she goes pink. It's the best and worst thing I've ever caused.
* [Say this was a mistake. #cautious]
    ~ temp r = check("magda_mistake", "tact")
    { r:
    - 0:
        ~ like("magda", -4)
        ~ remember("magda", "called_mistake")
        ~ flag("magda_hurt")
        ~ headline("Teo told Magda it was a mistake. Over her coffee.")
        She puts the mugs down very carefully. Like they might break, or she might.
        MAGDA: Okay. Okay. No. You're right. Drink your coffee, Teo. It's got sugar in.
    - else:
        ~ remember("magda", "gentle_mistake")
        ~ like("magda", -1)
        MAGDA: I know. I know, Teo. I'm twenty-one, not stupid.
        MAGDA: It was nice though. Wasn't it? Let's just say it was nice.
        TEO: It was nice.
        It was. That's the problem with it.
    }
* [Say nothing. Drink the coffee.]
    ~ remember("magda", "silent_morning")
    I drink the coffee. It has sugar in. I don't take sugar. I drink it all.
- -> phone_check


=== wake_kornels
# phase: wed_wake
# kind: scene
# when: slept_at == 3
# scene: kornel_flat
I wake up under a sheet with hospital corners. There's a bucket at my feet that says <i>BUCKET</i>.
{ hangover() >= 3:
    I've used the bucket. Someone has emptied the bucket. Someone has washed the bucket and put it back. The note is new. It says <i>BUCKET (CLEAN)</i>.
}
Kornel is in the kitchen, in a shirt and tie already, making breakfast. Eggs. Toast cut into triangles. Coffee in the one mug, and tea for himself in a measuring jug.
KORNEL: Good morning. I didn't know how you like your eggs, so I made them how I like mine, and I'll have the other eggs if they're wrong.
They're not wrong. They're perfect. They're the best eggs I've ever had and I want to cry, and I think it's the hangover. I hope it's the hangover.
* [Thank him. Properly. #kind]
    ~ like("kornel", 3)
    ~ remember("kornel", "thanked")
    ~ flag("kornel_friend")
    TEO: Kornel. Thank you. Really.
    He goes still, holding the spatula. Like a man who's heard a sound in the forest he's never heard before.
    KORNEL: You're welcome.
    KORNEL: You're very welcome, Teo.
* [Eat fast and get out.]
    ~ remember("kornel", "rushed_out")
    I eat in four minutes. I say thanks to the door, mostly.
    KORNEL: Do you want a lift? It's no trouble. I'm going the same way anyway. Okay. Okay. See you there.
- -> phone_check


=== wake_station
# phase: wed_wake
# kind: scene
# when: slept_at == 4
# scene: station
I wake up on a plastic mattress in a room with eight beds, seven men and one drain in the middle of the floor.
The man in the next bed is wearing a suit and one shoe. He nods at me, like we're on a business trip.
MAN: First time?
TEO: Is it obvious?
MAN: You're looking at the drain.
At half seven, a nurse gives me my clothes in a plastic bag, still damp, and an invoice for the night. Four thousand two hundred. Breakdown: bed, supervision, "hygienic intervention." I don't want to know.
The invoice will be posted to my employer. That's the law. The nurse says it like she's telling me the weather.
NURSE: Have a good day, Mr Halász. Maybe less river.
~ flag("station_invoice")
~ shift("appetite", -1, "station")
~ headline("The sobering-up station is sending the invoice to Teo's office.")
-> phone_check


=== phone_check
{ played("kornel_poem") > 0 && (is("filmed_kornel") || q("kornel_poem") == 0):
    Twelve notifications. The office group chat. Someone's posted a video of Kornel's poem with the word <i>LEGEND</i>. It's not clear which way they mean it. It's never clear.
    { is("filmed_kornel"):
        Under it, a laughing emoji from eight people. Including me, apparently, at 1:14 in the morning.
    }
    ~ flag("video_in_chat")
}
{ is("wrong_door"):
    A message from a number I don't know. Just a video. The landing. A man trying his key in the wrong door, with enormous concentration. Then a frying pan.
    Then: <i>my dad says ur fixing his balcony. i'll delete this when u do x</i>
    ~ flag("daughter_video")
    ~ headline("The neighbours' daughter is holding a video of Teo hostage.")
}
{ is("photo_of_viktor"):
    In my photos: a blue, blurry picture of two shapes in a corner of Club Schengen. I look at it for a long time. I don't delete it.
}
{ is("magda_paid") == false && remembers("magda", "didnt_pay"):
    A message from Magda, from 06:02: <i>dont worry about gift money!! i covered u xx</i>
    She covered me. Of course she did.
}
-> DONE


// ── The office ───────────────────────────────────────────────

=== wed_arrive
# phase: wed_office
# kind: enter
# priority: 100
The office on the morning after looks like a crime scene where everyone is both the victim and the suspect.
Grey faces. Sunglasses indoors. Someone has brought a family-sized bottle of mineral water and is drinking it with both hands.
Viktor is fresh. Viktor is shaved and tanned and has had a run. Viktor is a medical mystery.
Zdenka has been here since six.
{ q("slept_at") == 3:
    Kornel and I arrive together. Everyone notices. Nobody says anything. That's worse than saying something.
}
-> DONE


=== wed_magda
# phase: wed_office
# kind: hub
# where: magda
# choice: Magda's desk.
# duration: 15
The balloons are deflating. Somebody, probably Magda, has tied the saddest one to her monitor.
{
- is("slept_with_magda") && is("magda_hurt"):
    She doesn't look up. She's typing. She types very fast when she's upset. It sounds like rain on a car.
    MAGDA: Morning, Teo.
    Just that. Professional. I didn't know she could do professional. It's the most frightening thing I've seen her do.
    ~ like("magda", -1)
- is("slept_with_magda") && is("magda_hopeful"):
    She looks up and smiles. A little smile. A secret smile, in an office where nothing has ever been a secret.
    MAGDA: I saved you a pastry. From the good bakery. Not the one by the tram.
    Zdenka watches us from across the room. Zdenka has already written this down somewhere.
- is("slept_with_magda"):
    She smiles at me, carefully, the way you test a step in the dark.
    MAGDA: Morning. Did you get home all right? I mean. You know what I mean.
- q("slept_at") == 2:
    MAGDA: You found the coffee!
    She's pleased about the coffee. She's pleased I slept on her sofa. She's going to tell people. In a nice way. It'll be worse in a nice way.
- is("magda_left_with_someone"):
    She's wearing yesterday's earrings. She sees me see them.
    MAGDA: Don't. I had a nice time. I'm allowed a nice time.
    She is. I just wish it looked more like one.
    { is("magda_left_with_adrian"):
        On her desk there's a thick card, embossed. Kovál Development. Card like that is a message by itself.
    }
- remembers("magda", "sat_with_later"):
    MAGDA: Thank you. For sitting with me. At the bar.
    She says it quietly, without looking at me, and then loudly asks if anyone wants a coffee.
    ~ like("magda", 1)
- remembers("magda", "sat_with") || remembers("magda", "truth"):
    MAGDA: Thank you. For last night. On the bench.
    She says it quietly, without looking at me, and then loudly asks if anyone wants a coffee.
    ~ like("magda", 1)
- remembers("magda", "left_her") || remembers("magda", "teo_left_early"):
    MAGDA: Morning!
    Bright. Normal. She's over it, or she's decided I don't get to see if she's over it.
    She doesn't save me a pastry. There are pastries. I can see the box.
- remembers("magda", "joked_about_her"):
    MAGDA: Thirty-four tickets, Teo. I used every one.
    She says it like a joke. She's turned it into a joke. That's what she does with things people do to her.
- else:
    MAGDA: Wasn't it lovely? Wasn't it a lovely party?
    It wasn't. She knows it wasn't. She's going to say it was until it was.
}
-> DONE


=== wed_kornel
# phase: wed_office
# kind: hub
# where: kornel
# choice: Kornel.
# duration: 15
{
- is("kornel_friend"):
    Kornel has put a coffee on my desk. In a mug that says <i>WORLD'S OKAYEST COLLEAGUE</i>. He bought it this morning. He must have gone to the shop on purpose.
    KORNEL: It's a joke. It's an ironic joke. I don't think you're okayest.
    KORNEL: I think you're better than okayest.
    I've got a friend. I'm thirty-two and I appear to have made a friend, and he's Kornel.
- q("kornel_poem") == 2:
    Kornel is different today. He's sitting up straighter. People keep saying "Ode to Permits" to him as they walk past, and they mean it nicely, mostly.
    { is("kornel_first_beer"):
        KORNEL: I had a beer. My first. I don't think I'll have another. I might have another.
    }
    { is("stood_with_kornel"):
        KORNEL: You stood next to me. Nobody's ever stood next to me.
        He says it like he's reporting a fact for the minutes. Then he goes red to the ears.
    }
- is("filmed_kornel"):
    -> filmed
- q("kornel_poem") == 0 && played("kornel_poem") > 0:
    KORNEL: I've decided not to read any more poems at work. I've made a note.
    He shows me the note. It's on his hand.
    { is("zdenka_said_it_rhymed"):
        KORNEL: Zdenka said it rhymed. Zdenka doesn't say things.
    }
- else:
    KORNEL: Good morning, Teo. Did you have a nice evening? I had an adequate evening.
}
-> DONE

= filmed
Kornel's at his desk with his headphones on. He's not listening to anything. I can see the cable isn't plugged in.
He's seen the video. He's seen who sent the laughing emoji.
* [Apologise. #kind]
    ~ temp r = check("kornel_sorry", "tact")
    { r:
    - 0:
        KORNEL: It's fine. It's absolutely fine. It's a free country.
        It's not fine. And it's barely a free country.
    - else:
        ~ like("kornel", 2)
        ~ remember("kornel", "apology")
        He takes one headphone off.
        KORNEL: Thank you, Teo. That was a good apology. It had all the parts.
    }
* [Pretend nothing happened.]
    ~ like("kornel", -1)
    ~ shift("warmth", -1, "filmed_kornel")
    I walk past. He doesn't look up. Two men pretending at each other, ten metres apart, for the rest of our careers.
- -> DONE


=== wed_zdenka
# phase: wed_office
# kind: hub
# where: zdenka
# choice: Zdenka wants a word.
# when: file_seen or file_buried or remembers(zdenka, asked_to_bury) or saw_zdenka_napkin
# duration: 15
Zdenka doesn't call me over. She just looks at me until I'm there.
{
- is("file_seen"):
    The Petrovici file is on her desk. Not on the trolley. On her desk, squared to the edge with the metal ruler.
    ZDENKA: It fell to the bottom of the trolley yesterday. Files are very clumsy.
    ZDENKA: I've filed it today, instead of Thursday. Since it was in such a hurry to be noticed.
    ~ flag("file_filed_early")
    ~ headline("Zdenka filed the Petrovici report early. She knows why.")
    She doesn't say anything else. She doesn't report me. That's worse. Now she has something, and Zdenka keeps things.
- is("file_buried"):
    ZDENKA: The Petrovici file isn't on the trolley.
    TEO: Isn't it?
    ZDENKA: No. It's at the bottom of the trolley. Under eleven others. Which is not the same as on.
    She looks at me with those winter eyes.
    ZDENKA: I'll find it Thursday. I find everything Thursday.
- remembers("zdenka", "asked_to_bury"):
    ZDENKA: Your neighbour came here this morning. Seven-fifteen. With a sausage.
    ZDENKA: A whole sausage, Teo. For me. As if I'm a dog at a gate.
    ZDENKA: I gave it to the cleaner. Tell him that.
- else:
    ZDENKA: You were looking at me. Last night. When I was folding the napkin.
    TEO: Was I?
    ZDENKA: People don't look at me. It's one of the good things about being me. #mood:smile
    She smiles. It's the first time I've seen it. I'd prefer not to see it again.
}
-> DONE


=== wed_viktor
# phase: wed_office
# kind: interrupt
# at: 09:15
# priority: 50
VIKTOR: Teo. A moment.
{
- is("viktor_promise"):
    He closes the door. He never closes the door.
    VIKTOR: Last night. Good night. Good man.
    VIKTOR: The restructuring. It's real. September. I need people I can trust.
    VIKTOR: Can I trust you, Teo?
    He's not asking about the restructuring.
    ~ flag("hook_viktor")
- remembers("viktor", "saw_teo_with_ramona"):
    He doesn't close the door. He leans on the frame, smiling.
    VIKTOR: Did you get home all right? Ramona said you were very good company.
    VIKTOR: She doesn't say that about people.
    The smile doesn't move. Nothing about him moves. He's a photograph of a man smiling.
    ~ flag("hook_ramona")
- remembers("viktor", "sabotaged"):
    He doesn't call me in. He walks past my desk and puts down a pile of files. Forty, maybe. All the drainage appeals since 2019.
    VIKTOR: Thought you'd like a project. You like being thorough.
    He's smiling. The office is very quiet.
    ~ flag("hook_punished")
- is("station_invoice"):
    He's holding an envelope. Municipal Sobering-Up Station, Kollár Street. Co-financed by the European Union.
    VIKTOR: Teo, Teo, Teo.
    He's delighted. He's genuinely delighted. He hasn't been this happy since the minister went to prison.
    VIKTOR: We've all been there. Well. I haven't. But people have.
    ~ flag("hook_station")
- is("bench_dots_connected") && is("flagged_bench"):
    VIKTOR: That email about the bench. Good catch. Really.
    VIKTOR: I've talked to the people involved. It's all in order. So you can forget about it now.
    VIKTOR: You can forget about it, can't you, buddy?
    ~ flag("hook_bench")
- else:
    VIKTOR: Good party?
    TEO: Good party.
    VIKTOR: Good. Good. Back to it.
    That's it. That's the whole conversation. I was part of the furniture of his evening. Somehow that's a relief.
}
-> DONE


=== wed_stojan
# phase: wed_office
# kind: interrupt
# at: 09:10
# priority: 20
# when: stojan_promise or wrong_door
Reception calls. There's a man asking for "Mr Permit."
{
- is("told_stojan_no"):
    Stojan's in the lobby in a suit. A real one, from a wedding a long time ago. He's not here for me. He's asking for "the woman who measures."
    He's going round me. Of course he is. That's what you do with a man who said no.
- is("wrong_door"):
    Stojan's in the lobby with a cake. A whole cake. Dana made it.
    STOJAN: She says, for the door. No hard feelings. And for the balcony. Hard feelings if not.
    The receptionist looks at the cake, then at me, then writes something down.
- else:
    Stojan's in the lobby holding a sausage the length of my arm, wrapped in newspaper.
    STOJAN: For the woman who measures. To say thank you in advance.
    TEO: Stojan, you can't.
    STOJAN: It's a sausage, Teo. It's not a bribe. A bribe is money. This is culture.
    ~ flag("stojan_sausage")
}
-> DONE


=== wed_desk
# phase: wed_office
# kind: exit
# where: desk
# choice: Sit down at your desk and open your email.
# window: 09:30-11:30
# duration: 5
I sit down. I open my email. There are forty-one unread messages.
-> DONE


// ── The end of the beginning ─────────────────────────────────
// One hook, the strongest one, closes the prototype.

=== end_viktor
# phase: wed_end
# kind: scene
# priority: 50
# when: hook_viktor
At five to twelve, an email from Viktor. Subject: <i>September</i>. No text. Just a calendar invite, for a meeting with no name, in a room I didn't know we had.
I accept it. Of course I accept it.
On the other side of the room, Magda is watching Viktor's door. Zdenka is watching Magda. Kornel is watching me. Nobody is watching Zdenka.
-> finale


=== end_ramona
# phase: wed_end
# kind: scene
# priority: 45
# when: hook_ramona or kissed_ramona
At twelve, my phone buzzes. A number I don't have saved.
<i>Bogdan will collect you Friday at 8. Wear something you'd build in. R.</i>
I don't know what that means. I know what it means.
-> finale


=== end_daughter
# phase: wed_end
# kind: scene
# priority: 40
# when: daughter_video
At twelve, another message from the neighbours' daughter. A screenshot of the video, paused on my face, mid-concentration, key in hand.
<i>its doing numbers on my story. 300 views. fix the balcony mr permit</i>
I'm being blackmailed by a seventeen-year-old for a toilet. This is the most important I've ever been.
-> finale


=== end_station
# phase: wed_end
# kind: scene
# priority: 35
# when: hook_station or station_invoice
By lunch, the whole office knows. Somebody's printed the invoice and put it on the noticeboard, next to the fire drill instructions.
Underneath, in Magda's handwriting: <i>We love you Teo!! xx</i>. And under that, in Kornel's: <i>Hygienic intervention is a normal medical procedure.</i>
I've never been so famous. I've never been so loved. I want to die. It's going quite well.
-> finale


=== end_magda
# phase: wed_end
# kind: scene
# priority: 30
# when: slept_with_magda
At lunch, there's a note in my drawer. Neat handwriting. A little balloon drawn next to the x.
<i>Thursday? I'll cook. No pressure!! (some pressure) M x</i>
I put it back in the drawer. I take it out again. I put it in my wallet, next to a receipt from 2021 I keep for no reason.
-> finale


=== end_zdenka
# phase: wed_end
# kind: scene
# priority: 25
# when: file_filed_early
At twelve, Zdenka passes my desk on the way to lunch. She leaves a pencil on it. Sharpened. The long, careful way.
I don't know what it means. I'm going to think about it for a year.
-> finale


=== end_kornel
# phase: wed_end
# kind: scene
# priority: 20
# when: kornel_friend or slept_at == 3
At twelve, Kornel appears at my desk.
KORNEL: Lunch? It's soup. Both soups.
TEO: Yeah. Yeah, all right.
He looks so happy I have to look at the EU poster. The family on the bridge. They look happy too. They've always looked happy. I think I finally get it. They're just glad someone came with them.
-> finale


=== end_punished
# phase: wed_end
# kind: scene
# priority: 24
# when: hook_punished
By twelve I've read four drainage appeals. There are thirty-six to go. Viktor walks past at ten to, on his way to lunch with someone from the ministry, and pats the pile like it's a dog.
VIKTOR: Thorough, Teo. That's what I like about you.
Magda looks at me across the office with enormous sympathy. Zdenka doesn't look at me at all, which from Zdenka is almost the same thing.
-> finale


=== end_koval
# phase: wed_end
# kind: scene
# priority: 10
# weight: 3
# when: took_kovals_card
At twelve, a call from a number with a lot of zeros in it.
WOMAN: Mr Halász? Kovál Development. Mr Kovál asked me to invite you to a viewing. Saturday. Riverside Residences, the show flat. Just to look.
WOMAN: There's no obligation. There's coffee.
I haven't said yes. I haven't said no. She takes it as a yes. Everybody does.
-> finale


=== end_stojan
# phase: wed_end
# kind: scene
# priority: 10
# weight: 2
# when: file_photo or stojan_sausage or told_stojan_no or stojan_refused
{
- is("told_stojan_no") || is("stojan_refused"):
    At twelve, I see Stojan in the lobby again, through the glass. He's sitting very straight in the waiting area, in his wedding suit, with a folder on his knees.
    He's waiting for Zdenka. He's going to wait as long as it takes. He has the patience of a man who has built a toilet in the sky.
- is("file_photo"):
    At twelve, a message from Stojan. A photo of the balcony, from below, with a bus stop in it. He's drawn an arrow on it.
    <i>bus stop is very far, they exaggerate. thank you for information Teo. you are good neighbour. we will discuss</i>
    We will discuss. In Kaslovia that's not a sentence. It's a sentence.
- else:
    At twelve, Zdenka walks past my desk carrying the sausage, wrapped in newspaper, to the cleaner.
    She doesn't say anything. She doesn't need to. The sausage says it.
}
-> finale


=== end_tower
# phase: wed_end
# kind: scene
# priority: 10
# weight: 3
# when: approved_tower
At twelve, an email from Lower Varosh. Subject: <i>THANK YOU!!!!</i>
No text. One photo. A man on the roof of his garage, standing on some scaffolding where an eleven-metre tower is going to be, waving at the camera with both arms.
I don't know if I've done a good thing. I print it out. I put it on my wall. It's the first thing I've ever put on my wall.
-> finale


=== end_poem
# phase: wed_end
# kind: scene
# priority: 10
# weight: 3
# when: kornel_poem == 2
At twelve, I notice it on the noticeboard: <i>Ode to Permits</i>, typed, double-spaced, pinned up between the fire drill and the canteen menu.
Somebody's pinned it up with care. Four pins, one in each corner, perfectly square.
Only one person in this building uses four pins.
-> finale


=== end_video
# phase: wed_end
# kind: scene
# priority: 10
# weight: 3
# when: video_in_chat
At twelve, an email to all staff from Human Resources. Subject: <i>Respectful Use of Group Messaging</i>.
It doesn't mention the video. It mentions "dignity" twice. It's signed by Viktor, who sent the second laughing emoji.
Kornel reads it at his desk. He reads it all the way through. Then he prints it out, and files it.
-> finale


=== end_consultants
# phase: wed_end
# kind: scene
# priority: 10
# weight: 3
# when: played_local
At twelve, a LinkedIn notification. Felicity, from Brussels Advisory Partners, has tagged me in a post.
<i>Humbled to learn about the ancient Kaslovian tradition of drinking from a shoe. Authentic local knowledge is what co-creation is all about.</i>
Four hundred likes. Someone from the Ministry of Culture has commented: <i>Proud of our heritage!</i>
It's in the deck now. It's going to be in a museum by Christmas.
-> finale


=== end_default
# phase: wed_end
# kind: scene
# priority: 0
At twelve, an email to all staff from Human Resources. Subject: <i>Summer Party: Feedback Form</i>.
Question one: <i>On a scale of 1 to 5, how would you rate the evening?</i>
I sit there for a long time with the cursor over the numbers. There should be a box for "it happened."
-> finale


=== finale
{ is("took_kovals_card"):
    Kovál's card is still in my wallet. Thick card. Embossed. It hasn't done anything yet. Neither have I.
}
{ is("flagged_bench") && not is("hook_bench"):
    The Innovation Bench application is still in my inbox, approved by someone, with a note from Viktor: <i>Sorted. V.</i>
}
It's Wednesday. Twelve o'clock. The summer party was yesterday, and it's already a story people are telling, and I'm in it.
I'm in it.
~ goto_phase("end")
-> DONE
