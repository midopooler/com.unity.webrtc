﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Unity.WebRTC
{
    public class StringValueAttribute : Attribute
    {
        public string StringValue { get; protected set; }

        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }
    }

    public enum RTCStatsType
    {
        [StringValue("codec")]
        Codec = 0,
        [StringValue("inbound-rtp")]
        InboundRtp = 1,
        [StringValue("outbound-rtp")]
        OutboundRtp = 2,
        [StringValue("remote-inbound-rtp")]
        RemoteInboundRtp = 3,
        [StringValue("remote-outbound-rtp")]
        RemoteOutboundRtp = 4,
        [StringValue("media-source")]
        MediaSource = 5,
        [StringValue("csrc")]
        Csrc = 6,
        [StringValue("peer-connection")]
        PeerConnection = 7,
        [StringValue("data-channel")]
        DataChannel = 8,
        [StringValue("stream")]
        Stream = 9,
        [StringValue("track")]
        Track = 10,
        [StringValue("transceiver")]
        Transceiver = 11,
        [StringValue("sender")]
        Sender = 12,
        [StringValue("receiver")]
        Receiver = 13,
        [StringValue("transport")]
        Transport = 14,
        [StringValue("sctp-transport")]
        SctpTransport = 15,
        [StringValue("candidate-pair")]
        CandidatePair = 16,
        [StringValue("local-candidate")]
        LocalCandidate = 17,
        [StringValue("remote-candidate")]
        RemoteCandidate = 18,
        [StringValue("certificate")]
        Certificate = 19,
        [StringValue("ice-server")]
        IceServer = 20,
    }

    internal enum StatsMemberType
    {
        Bool,    // bool
        Int32,   // int32_t
        Uint32,  // uint32_t
        Int64,   // int64_t
        Uint64,  // uint64_t
        Double,  // double
        String,  // std::string

        SequenceBool,    // std::vector<bool>
        SequenceInt32,   // std::vector<int32_t>
        SequenceUint32,  // std::vector<uint32_t>
        SequenceInt64,   // std::vector<int64_t>
        SequenceUint64,  // std::vector<uint64_t>
        SequenceDouble,  // std::vector<double>
        SequenceString,  // std::vector<std::string>
    }

    internal class RTCStatsMember
    {
        internal IntPtr self;

        internal RTCStatsMember(IntPtr ptr)
        {
            self = ptr;
        }

        internal string GetName()
        {
            return NativeMethods.StatsMemberGetName(self).AsAnsiStringWithFreeMem();
        }

        internal StatsMemberType GetValueType()
        {
            return NativeMethods.StatsMemberGetType(self);
        }

        internal object GetValue()
        {
            StatsMemberType type = this.GetValueType();

            UnityEngine.Debug.LogFormat("{0} {1}", GetName(), type);

            uint length = 0;
            switch (type)
            {
                case StatsMemberType.Bool:
                    return NativeMethods.StatsMemberGetBool(self);
                case StatsMemberType.Int32:
                    return NativeMethods.StatsMemberGetInt(self);
                case StatsMemberType.Uint32:
                    return NativeMethods.StatsMemberGetUnsignedInt(self);
                case StatsMemberType.Int64:
                    return NativeMethods.StatsMemberGetLong(self);
                case StatsMemberType.Uint64:
                    return NativeMethods.StatsMemberGetUnsignedLong(self);
                case StatsMemberType.Double:
                    return NativeMethods.StatsMemberGetDouble(self);
                case StatsMemberType.String:
                    return NativeMethods.StatsMemberGetString(self).AsAnsiStringWithFreeMem();
                case StatsMemberType.SequenceBool:
                    return NativeMethods.StatsMemberGetBoolArray(self, ref length).AsBoolArray((int)length);
                case StatsMemberType.SequenceInt32:
                    return NativeMethods.StatsMemberGetIntArray(self, ref length).AsIntArray((int)length);
                case StatsMemberType.SequenceUint32:
                    return NativeMethods.StatsMemberGetUnsignedIntArray(self, ref length).AsUnsignedIntArray((int)length);
                case StatsMemberType.SequenceInt64:
                    return NativeMethods.StatsMemberGetLongArray(self, ref length).AsLongArray((int)length);
                case StatsMemberType.SequenceUint64:
                    return NativeMethods.StatsMemberGetUnsignedLongArray(self, ref length).AsUnsignedLongArray((int)length);
                case StatsMemberType.SequenceDouble:
                    return NativeMethods.StatsMemberGetDoubleArray(self, ref length).AsDoubleArray((int)length);
                case StatsMemberType.SequenceString:
                    return NativeMethods.StatsMemberGetStringArray(self, ref length).AsStringArray((int)length);
                default:
                    throw new ArgumentException();
            }
        }
    }

    public class RTCStats : IReadOnlyDictionary<string, object>
    {
        private IntPtr self;
        internal Dictionary<string, RTCStatsMember> m_members;

        public RTCStatsType Type
        {
            get
            {
                return NativeMethods.StatsGetType(self);
            }
        }

        public string Id
        {
            get { return NativeMethods.StatsGetId(self).AsAnsiStringWithFreeMem(); }
        }

        public long Timestamp
        {
            get { return NativeMethods.StatsGetTimestamp(self); }
        }

        public bool ContainsKey(string key)
        {
            return m_members.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            RTCStatsMember member;
            if (!m_members.TryGetValue(key, out member))
            {
                value = null;
                return false;
            }
            value = member.GetValue();
            return true;
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return m_members.Keys;
            }
        }
        public IEnumerable<object> Values
        {
            get
            {
                return m_members.Values.Select(member => member.GetValue());
            }
        }

        public int Count
        {
            get
            {
                return m_members.Count;
            }
        }

        public object this[string key]
        {
            get
            {
                return m_members[key].GetValue();
            }
        }

        public bool GetBool(string key)
        {
            return NativeMethods.StatsMemberGetBool(m_members[key].self);
        }
        public int GetInt(string key)
        {
            return NativeMethods.StatsMemberGetInt(m_members[key].self);
        }
        public uint GetUnsignedInt(string key)
        {
            return NativeMethods.StatsMemberGetUnsignedInt(m_members[key].self);
        }
        public long GetLong(string key)
        {
            return NativeMethods.StatsMemberGetLong(m_members[key].self);
        }
        public ulong GetUnsignedLong(string key)
        {
            return NativeMethods.StatsMemberGetUnsignedLong(m_members[key].self);
        }
        public double GetDouble(string key)
        {
            return NativeMethods.StatsMemberGetDouble(m_members[key].self);
        }
        public string GetString(string key)
        {
            return NativeMethods.StatsMemberGetString(m_members[key].self).AsAnsiStringWithFreeMem();
        }
        public bool[] GetBoolArray(string key)
        {
            uint length = 0;
            return NativeMethods.StatsMemberGetBoolArray(m_members[key].self, ref length).AsBoolArray((int)length);
        }
        public int[] GetIntArray(string key)
        {
            uint length = 0;
            return NativeMethods.StatsMemberGetIntArray(m_members[key].self, ref length).AsIntArray((int)length);
        }

        public uint[] GetUnsignedIntArray(string key)
        {
            uint length = 0;
            return NativeMethods.StatsMemberGetUnsignedIntArray(m_members[key].self, ref length).AsUnsignedIntArray((int)length);
        }
        public long[] GetLongArray(string key)
        {
            uint length = 0;
            return NativeMethods.StatsMemberGetLongArray(m_members[key].self, ref length).AsLongArray((int)length);
        }
        public ulong[] GetUnsignedLongArray(string key)
        {
            uint length = 0;
            return NativeMethods.StatsMemberGetUnsignedLongArray(m_members[key].self, ref length).AsUnsignedLongArray((int)length);
        }
        public double[] GetDoubleArray(string key)
        {
            uint length = 0;
            return NativeMethods.StatsMemberGetDoubleArray(m_members[key].self, ref length).AsDoubleArray((int)length);
        }
        public string[] GetStringArray(string key)
        {
            uint length = 0;
            return NativeMethods.StatsMemberGetStringArray(m_members[key].self, ref length).AsStringArray((int)length);
        }

        class MemberEnumerator : IEnumerator<KeyValuePair<string, object>>
        {
            Dictionary<string, RTCStatsMember> m_members;
            int m_pos;

            public MemberEnumerator(Dictionary<string, RTCStatsMember> members)
            {
                m_members = members;
                m_pos = -1;
            }

            public KeyValuePair<string, object> Current
            {
                get
                {
                    var member = m_members.ElementAt(m_pos);
                    return new KeyValuePair<string, object>(member.Key, member.Value.GetValue());
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (object)this.Current;
                }
            }

            public bool MoveNext()
            {
                m_pos++;
                if (m_pos >= m_members.Count)
                {
                    return false;
                }
                return true;
            }
            public void Reset()
            {
                m_pos = -1;
            }
            void IDisposable.Dispose()
            {
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return new MemberEnumerator(m_members);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal RTCStats(IntPtr ptr)
        {
            self = ptr;
            RTCStatsMember[] array = GetMembers();
            m_members = new Dictionary<string, RTCStatsMember>();
            foreach (var member in array)
            {
                m_members.Add(member.GetName(), member);
            }
        }

        RTCStatsMember[] GetMembers()
        {
            uint length = 0;
            IntPtr ptr = NativeMethods.StatsGetMembers(self, ref length);
            IntPtr[] array = ptr.AsArray<IntPtr>((int)length);

            RTCStatsMember[] members = new RTCStatsMember[length];
            for (int i = 0; i < length; i++)
            {
                members[i] = new RTCStatsMember(array[i]);
            }

            return members;
        }

        public string ToJson()
        {
            return NativeMethods.StatsGetJson(self).AsAnsiStringWithFreeMem();
        }
    }

    public class RTCRtpStreamStats : RTCStats
    {
        internal RTCRtpStreamStats(IntPtr ptr) : base(ptr)
        {
        }

        public string CodecId
        {
            get { return ""; }
        }
    }

    public class RTCInboundRtpStreamStats : RTCStats
    {
        internal RTCInboundRtpStreamStats(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class RTCCertificateStats : RTCStats
    {
        public string fingerprint { get { return GetString("fingerprint"); } }
        public string fingerprintAlgorithm { get { return GetString("fingerprintAlgorithm"); } }
        public string base64Certificate { get { return GetString("base64Certificate"); } }
        public string issuerCertificateId { get { return GetString("issuerCertificateId"); } }
        internal RTCCertificateStats(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class RTCCodecStats : RTCStats
    {
        public uint payloadType { get { return GetUnsignedInt("payloadType"); } }
        public string mimeType { get { return GetString("mimeType"); } }
        public uint clockRate { get { return GetUnsignedInt("clockRate"); } }
        public uint channels { get { return GetUnsignedInt("channels"); } }
        public string sdpFmtpLine { get { return GetString("sdpFmtpLine"); } }
        internal RTCCodecStats(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class RTCIceCandidatePairStats : RTCStats
    {
        public string transportId { get { return GetString("transportId"); } }
        public string localCandidateId { get { return GetString("localCandidateId"); } }
        public string remoteCandidateId { get { return GetString("remoteCandidateId"); } }
        public string state { get { return GetString("state"); } }
        public ulong priority { get { return GetUnsignedLong("priority"); } }
        public bool nominated { get { return GetBool("nominated"); } }
        public bool writable { get { return GetBool("writable"); } }
        public bool readable { get { return GetBool("readable"); } }
        public ulong bytesSent { get { return GetUnsignedInt("bytesSent"); } }
        public ulong bytesReceived { get { return GetUnsignedInt("bytesReceived"); } }
        public double totalRoundTripTime { get { return GetUnsignedInt("totalRoundTripTime"); } }
        public double currentRoundTripTime { get { return GetUnsignedInt("currentRoundTripTime"); } }
        public double availableOutgoingBitrate { get { return GetUnsignedInt("availableOutgoingBitrate"); } }
        public double availableIncomingBitrate { get { return GetUnsignedInt("availableIncomingBitrate"); } }
        public ulong requestsReceived { get { return GetUnsignedInt("requestsReceived"); } }
        public ulong requestsSent { get { return GetUnsignedInt("requestsSent"); } }
        public ulong responsesReceived { get { return GetUnsignedInt("responsesReceived"); } }
        public ulong responsesSent { get { return GetUnsignedInt("responsesSent"); } }
        public ulong retransmissionsReceived { get { return GetUnsignedInt("retransmissionsReceived"); } }
        public ulong retransmissionsSent { get { return GetUnsignedInt("retransmissionsSent"); } }
        public ulong consentRequestsReceived { get { return GetUnsignedInt("consentRequestsReceived"); } }
        public ulong consentRequestsSent { get { return GetUnsignedInt("consentRequestsSent"); } }
        public ulong consentResponsesReceived { get { return GetUnsignedInt("consentResponsesReceived"); } }
        public ulong consentResponsesSent { get { return GetUnsignedInt("consentResponsesSent"); } }

        internal RTCIceCandidatePairStats(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class RTCIceCandidateStats : RTCStats
    {
        public string transportId { get { return GetString("transportId"); } }
        public bool isRemote { get { return GetBool("isRemote"); } }
        public string networkType { get { return GetString("networkType"); } }
        public string ip { get { return GetString("ip"); } }
        public int port { get { return GetInt("port"); } }
        public string protocol { get { return GetString("protocol"); } }
        public string relayProtocol { get { return GetString("relayProtocol"); } }
        public string candidateType { get { return GetString("candidateType"); } }
        public int priority { get { return GetInt("priority"); } }
        public string url { get { return GetString("url"); } }
        public bool deleted { get { return GetBool("deleted"); } }

        internal RTCIceCandidateStats(IntPtr ptr) : base(ptr)
        {
        }

    }


    public class RTCInboundRTPStreamStats : RTCRTPStreamStats
    {
        public uint packetsReceived { get { return GetUnsignedInt("packetsReceived"); } }
        public ulong bytesReceived { get { return GetUnsignedLong("bytesReceived"); } }
        public ulong headerBytesReceived { get { return GetUnsignedLong("headerBytesReceived"); } }
        public int packetsLost { get { return GetInt("packetsLost"); } }
        public double lastPacketReceivedTimestamp { get { return GetDouble("lastPacketReceivedTimestamp"); } }
        public double jitter { get { return GetDouble("jitter"); } }
        public double roundTripTime { get { return GetDouble("roundTripTime"); } }
        public uint packetsDiscarded { get { return GetUnsignedInt("packetsDiscarded"); } }
        public uint packetsRepaired { get { return GetUnsignedInt("packetsRepaired"); } }
        public uint burstPacketsLost { get { return GetUnsignedInt("burstPacketsLost"); } }
        public uint burstPacketsDiscarded { get { return GetUnsignedInt("burstPacketsDiscarded"); } }
        public uint burstLossCount { get { return GetUnsignedInt("burstLossCount"); } }
        public uint burstDiscardCount { get { return GetUnsignedInt("burstDiscardCount"); } }
        public double burstLossRate { get { return GetDouble("burstLossRate"); } }
        public double burstDiscardRate { get { return GetDouble("burstDiscardRate"); } }
        public double gapLossRate { get { return GetDouble("gapLossRate"); } }
        public double gapDiscardRate { get { return GetDouble("gapDiscardRate"); } }
        public uint framesDecoded { get { return GetUnsignedInt("framesDecoded"); } }
        public uint keyFramesDecoded { get { return GetUnsignedInt("keyFramesDecoded"); } }
        public double totalDecodeTime { get { return GetDouble("totalDecodeTime"); } }
        public string contentType { get { return GetString("contentType"); } }
        public string decoderImplementation { get { return GetString("decoderImplementation"); } }

        internal RTCInboundRTPStreamStats(IntPtr ptr) : base(ptr)
        {
        }
    }
    public class RTCMediaStreamTrackStats : RTCStats
    {
        public string trackIdentifier { get { return GetString("trackIdentifier"); } }
        public string mediaSourceId { get { return GetString("mediaSourceId"); } }
        public bool remoteSource { get { return GetBool("remoteSource"); } }
        public bool ended { get { return GetBool("ended"); } }
        public bool detached { get { return GetBool("detached"); } }
        public string kind { get { return GetString("kind"); } }
        public double jitterBufferDelay { get { return GetDouble("jitterBufferDelay"); } }
        public ulong jitterBufferEmittedCount { get { return GetUnsignedLong("jitterBufferEmittedCount"); } }
        public uint frameWidth { get { return GetUnsignedInt("frameWidth"); } }
        public uint frameHeight { get { return GetUnsignedInt("frameHeight"); } }
        public double framesPerSecond { get { return GetDouble("framesPerSecond"); } }
        public uint framesSent { get { return GetUnsignedInt("framesSent"); } }
        public uint hugeFramesSent { get { return GetUnsignedInt("hugeFramesSent"); } }
        public uint framesReceived { get { return GetUnsignedInt("framesReceived"); } }
        public uint framesDecoded { get { return GetUnsignedInt("framesDecoded"); } }
        public uint framesDropped { get { return GetUnsignedInt("framesDropped"); } }
        public uint framesCorrupted { get { return GetUnsignedInt("framesCorrupted"); } }
        public uint partialFramesLost { get { return GetUnsignedInt("partialFramesLost"); } }
        public uint fullFramesLost { get { return GetUnsignedInt("decoderImplementation"); } }
        public double audioLevel { get { return GetDouble("audioLevel"); } }
        public double totalAudioEnergy { get { return GetDouble("totalAudioEnergy"); } }
        public double echoReturnLoss { get { return GetDouble("echoReturnLoss"); } }
        public double echoReturnLossEnhancement { get { return GetDouble("echoReturnLossEnhancement"); } }
        public ulong totalSamplesReceived { get { return GetUnsignedLong("totalSamplesReceived"); } }
        public double totalSamplesDuration { get { return GetDouble("totalSamplesDuration"); } }
        public ulong concealedSamples { get { return GetUnsignedLong("concealedSamples"); } }
        public ulong silentConcealedSamples { get { return GetUnsignedLong("silentConcealedSamples"); } }
        public ulong concealmentEvents { get { return GetUnsignedLong("concealmentEvents"); } }
        public ulong insertedSamplesForDeceleration { get { return GetUnsignedLong("insertedSamplesForDeceleration"); } }
        public ulong removedSamplesForAcceleration { get { return GetUnsignedLong("removedSamplesForAcceleration"); } }
        public ulong jitterBufferFlushes { get { return GetUnsignedLong("jitterBufferFlushes"); } }
        public ulong delayedPacketOutageSamples { get { return GetUnsignedLong("delayedPacketOutageSamples"); } }
        public double relativePacketArrivalDelay { get { return GetDouble("relativePacketArrivalDelay"); } }
        public uint interruptionCount { get { return GetUnsignedInt("interruptionCount"); } }
        public double totalInterruptionDuration { get { return GetDouble("totalInterruptionDuration"); } }
        public uint freezeCount { get { return GetUnsignedInt("freezeCount"); } }
        public uint pauseCount { get { return GetUnsignedInt("pauseCount"); } }
        public double totalFreezesDuration { get { return GetDouble("totalFreezesDuration"); } }
        public double totalPausesDuration { get { return GetDouble("totalPausesDuration"); } }
        public double totalFramesDuration { get { return GetDouble("totalFramesDuration"); } }
        public double sumOfSquaredFramesDuration { get { return GetDouble("sumOfSquaredFramesDuration"); } }

        internal RTCMediaStreamTrackStats(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class MediaStreamStats : RTCStats
    {
        public string streamIdentifier { get { return GetString("streamIdentifier"); } }
        public string[] trackIds { get { return GetStringArray("trackIds"); } }

        internal MediaStreamStats(IntPtr ptr) : base(ptr)
        {
        }

    }

    public class RTCRTPStreamStats : RTCStats
    {
        public uint ssrc { get { return GetUnsignedInt("ssrc"); } }
        public string associateStatsId { get { return GetString("associateStatsId"); } }
        public bool isRemote { get { return GetBool("isRemote"); } }
        public string mediaType { get { return GetString("mediaType"); } }
        public string kind { get { return GetString("kind"); } }
        public string trackId { get { return GetString("trackId"); } }
        public string transportId { get { return GetString("transportId"); } }
        public string codecId { get { return GetString("codecId"); } }
        public uint firCount { get { return GetUnsignedInt("firCount"); } }
        public uint pliCount { get { return GetUnsignedInt("pliCount"); } }
        public uint nackCount { get { return GetUnsignedInt("nackCount"); } }
        public uint sliCount { get { return GetUnsignedInt("sliCount"); } }
        public ulong qpSum { get { return GetUnsignedLong("qpSum"); } }

        internal RTCRTPStreamStats(IntPtr ptr) : base(ptr)
        {
        }

    }

    public class RTCOutboundRTPStreamStats : RTCRTPStreamStats
    {
        mediaSourceId String
        packetsSent Uint32
        retransmittedPacketsSent Uint64
        bytesSent Uint64
        headerBytesSent Uint64
        retransmittedBytesSent Uint64
        targetBitrate Double
        framesEncoded Uint32
        keyFramesEncoded Uint32
        totalEncodeTime Double
        totalEncodedBytesTarget Uint64
        totalPacketSendDelay Double
        qualityLimitationReason String
        qualityLimitationResolutionChanges Uint32
        contentType String
        encoderImplementation String

        internal RTCOutboundRTPStreamStats(IntPtr ptr) : base(ptr)
        {
        }
    }
    public class RTCRemoteInboundRtpStreamStats : RTCStats
    {
        internal RTCRemoteInboundRtpStreamStats(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class RTCRemoteOutboundRtpStreamStats : RTCStats
    {
        internal RTCRemoteOutboundRtpStreamStats(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class RTCRemoteRTPStreamStats : RTCStats
    {
        internal RTCRemoteRTPStreamStats(IntPtr ptr) : base(ptr)
        {

        }
    }

    public class RTCMediaSourceStats : RTCStats
    {
        internal RTCMediaSourceStats(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class RTCPeerConnectionStats : RTCStats
    {
        public ulong dataChannelsOpened { get { return (ulong)base["dataChannelsOpened"]; } }
        public ulong dataChannelsClosed { get { return (ulong)base["dataChannelsClosed"]; } }
        public ulong dataChannelsRequested { get { return (ulong)base["dataChannelsRequested"]; } }
        public ulong dataChannelsAccepted { get { return (ulong)base["dataChannelsAccepted"]; } }

        internal RTCPeerConnectionStats(IntPtr ptr) : base(ptr)
        {
        }
    }

    internal class StatsFactory
    {
        static Dictionary<RTCStatsType, Func<IntPtr, RTCStats>> m_map;

        static StatsFactory()
        {
            m_map = new Dictionary<RTCStatsType, Func<IntPtr, RTCStats>>()
            {
                { RTCStatsType.Codec, ptr => new RTCCodecStats(ptr) },
                { RTCStatsType.InboundRtp, ptr => new RTCInboundRTPStreamStats(ptr) },
                { RTCStatsType.OutboundRtp, ptr => new RTCOutboundRTPStreamStats(ptr) },
                { RTCStatsType.RemoteInboundRtp, ptr => new RTCRemoteInboundRtpStreamStats(ptr) },
                { RTCStatsType.RemoteOutboundRtp, ptr => new RTCRemoteOutboundRtpStreamStats(ptr) },
                { RTCStatsType.MediaSource, ptr => new RTCMediaSourceStats(ptr) },
                { RTCStatsType.Csrc, ptr => new RTCCodecStats(ptr) },
                { RTCStatsType.PeerConnection, ptr => new RTCCodecStats(ptr) },
                { RTCStatsType.DataChannel, ptr => new RTCCodecStats(ptr) },
                { RTCStatsType.Stream, ptr => new RTCMediaStreamStats(ptr) },
                { RTCStatsType.Track, ptr => new RTCMediaStreamTrackStats(ptr) },
                { RTCStatsType.Transceiver, ptr => new RTCCodecStats(ptr) },
                { RTCStatsType.Sender, ptr => new RTCCodecStats(ptr) },
                { RTCStatsType.Receiver, ptr => new RTCCodecStats(ptr) },
                { RTCStatsType.Transport, ptr => new RTCCodecStats(ptr) },
                { RTCStatsType.SctpTransport, ptr => new RTCCodecStats(ptr) },
                { RTCStatsType.CandidatePair, ptr => new RTCIceCandidatePairStats(ptr) },
                { RTCStatsType.LocalCandidate, ptr => new RTCIceCandidateStats(ptr) },
                { RTCStatsType.RemoteCandidate, ptr => new RTCCodecStats(ptr) },
                { RTCStatsType.Certificate, ptr => new RTCCertificateStats(ptr) },
            };
        }

        public static RTCStats Create(RTCStatsType type, IntPtr ptr)
        {
            return m_map[type](ptr);
        }
    }

    public class RTCStatsReport : IReadOnlyDictionary<RTCStatsType, RTCStats>
    {
        private IntPtr self;
        private readonly Dictionary<RTCStatsType, RTCStats> m_dictStats;

        internal RTCStatsReport(IntPtr ptr)
        {
            self = ptr;
            uint length = 0;
            IntPtr ptrStatsTypeArray = IntPtr.Zero;
            IntPtr ptrStatsArray = NativeMethods.StatsReportGetStatsList(self, ref length, ref ptrStatsTypeArray);

            IntPtr[] array = ptrStatsArray.AsArray<IntPtr>((int)length);
            byte[] types = ptrStatsTypeArray.AsArray<byte>((int)length);

            m_dictStats = new Dictionary<RTCStatsType, RTCStats>();
            for (int i = 0; i < length; i++)
            {
                RTCStatsType type = (RTCStatsType)types[i];
                m_dictStats[type] = StatsFactory.Create(type, array[i]);
            }
        }

        public bool ContainsKey(RTCStatsType key)
        {
            return m_dictStats.ContainsKey(key);
        }

        public bool TryGetValue(RTCStatsType key, out RTCStats value)
        {
            return m_dictStats.TryGetValue(key, out value);
        }

        public IEnumerable<RTCStatsType> Keys
        {
            get
            {
                return m_dictStats.Keys;
            }
        }
        public IEnumerable<RTCStats> Values
        {
            get
            {
                return m_dictStats.Values;
            }
        }

        public int Count
        {
            get
            {
                return m_dictStats.Count;
            }
        }

        public RTCStats this[RTCStatsType key]
        {
            get
            {
                return m_dictStats[key];
            }
        }

        public IEnumerator<KeyValuePair<RTCStatsType, RTCStats>> GetEnumerator()
        {
            return m_dictStats.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
