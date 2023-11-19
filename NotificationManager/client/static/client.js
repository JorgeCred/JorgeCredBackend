const publicVapidKey =
    'BBa8uidwHniYRXzy0Wsaz3Ne7MjuU9ghqvxrz92jxA_eFOskU6EEwTigU3ySJterCgZuW-ohp3TXrI3A-miBcNg'

if ('serviceWorker' in navigator) {
    send().catch((err) => console.error(err))
}

async function send() {
    // Register service worker
    console.log('Registering Service Worker...')
    const register = await navigator.serviceWorker.register('/worker.js', {
        scope: '/',
    })
    console.log('Service Worker Registered.')

    // Subscribe user push
    console.log('Registering Push...')
    const subscription = await register.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: urlBase64ToUint8Array(publicVapidKey),
    })

    let payload = subscription.toJSON()
    payload.title = 'Your Notification Title'
    payload.body = 'Your Notification Body'

    console.log('Adding push to queue...')
    await fetch('/send', {
        method: 'POST',
        body: JSON.stringify(payload),
        headers: {
            'Content-Type': 'application/json',
        },
    }).then(() => console.log('Push in queue.'))
}

function urlBase64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - (base64String.length % 4)) % 4)
    const base64 = (base64String + padding)
        .replace(/\-/g, '+')
        .replace(/_/g, '/')

    const rawData = window.atob(base64)
    const outputArray = new Uint8Array(rawData.length)

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i)
    }
    return outputArray
}
